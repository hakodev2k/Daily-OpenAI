import json
import os
import pathlib
import subprocess
import sys
import tempfile
import time
import unittest

SCRIPT = pathlib.Path(__file__).resolve().parents[1] / "scripts" / "process_guard.py"


class ProcessGuardTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.root = pathlib.Path(self.tmp.name)
        self.policy = self.root / "policy.json"
        self.registry = self.root / "registry.json"
        self.policy.write_text(json.dumps({
            "version": 1,
            "lease_seconds": 1,
            "stale_lease_grace_seconds": 0,
            "registry_path": str(self.registry),
        }), encoding="utf-8")
        self.children = []

    def tearDown(self):
        for p in self.children:
            if p.poll() is None:
                p.terminate()
                try:
                    p.wait(timeout=2)
                except subprocess.TimeoutExpired:
                    p.kill()
        self.tmp.cleanup()

    def run_guard(self, *args):
        return subprocess.run([sys.executable, str(SCRIPT), "--policy", str(self.policy), *args], text=True, capture_output=True)

    def spawn(self):
        p = subprocess.Popen([sys.executable, "-c", "import time; time.sleep(60)"])
        self.children.append(p)
        return p

    def test_register_and_inspect_live_identity(self):
        p = self.spawn()
        r = self.run_guard("register", "--task-id", "t1", "--pid", str(p.pid), "--nonce", "n1")
        self.assertEqual(r.returncode, 0, r.stderr)
        r = self.run_guard("inspect", "--task-id", "t1")
        self.assertEqual(r.returncode, 0, r.stdout + r.stderr)
        self.assertTrue(json.loads(r.stdout)["identity"]["match"])

    def test_completion_gate_blocks_live_owned_process(self):
        p = self.spawn()
        self.assertEqual(self.run_guard("register", "--task-id", "root", "--pid", str(p.pid), "--nonce", "n1").returncode, 0)
        r = self.run_guard("gate", "--task-id", "root")
        self.assertEqual(r.returncode, 3)
        self.assertFalse(json.loads(r.stdout)["ok"])

    def test_completion_gate_passes_after_process_exits(self):
        p = subprocess.Popen([sys.executable, "-c", "pass"])
        self.children.append(p)
        self.assertEqual(self.run_guard("register", "--task-id", "root", "--pid", str(p.pid), "--nonce", "n1").returncode, 0)
        p.wait(timeout=2)
        r = self.run_guard("gate", "--task-id", "root")
        self.assertEqual(r.returncode, 0, r.stdout + r.stderr)
        self.assertTrue(json.loads(r.stdout)["ok"])

    def test_stale_lease_detected(self):
        p = self.spawn()
        self.assertEqual(self.run_guard("register", "--task-id", "t1", "--pid", str(p.pid), "--nonce", "n1").returncode, 0)
        data = json.loads(self.registry.read_text(encoding="utf-8"))
        data["tasks"]["t1"]["heartbeat_epoch"] = time.time() - 10
        self.registry.write_text(json.dumps(data), encoding="utf-8")
        r = self.run_guard("stale")
        self.assertEqual(r.returncode, 0)
        self.assertEqual(json.loads(r.stdout)["count"], 1)

    def test_pid_identity_mismatch_fails_closed(self):
        p = self.spawn()
        self.assertEqual(self.run_guard("register", "--task-id", "t1", "--pid", str(p.pid), "--nonce", "n1").returncode, 0)
        data = json.loads(self.registry.read_text(encoding="utf-8"))
        data["tasks"]["t1"]["start_identity"] = "definitely-wrong"
        self.registry.write_text(json.dumps(data), encoding="utf-8")
        r = self.run_guard("inspect", "--task-id", "t1")
        self.assertEqual(r.returncode, 2)
        self.assertFalse(json.loads(r.stdout)["identity"]["match"])

    def test_child_blocks_parent_gate(self):
        parent = subprocess.Popen([sys.executable, "-c", "pass"])
        self.children.append(parent)
        self.assertEqual(self.run_guard("register", "--task-id", "root", "--pid", str(parent.pid), "--nonce", "r").returncode, 0)
        child = self.spawn()
        self.assertEqual(self.run_guard("register", "--task-id", "child", "--parent-id", "root", "--pid", str(child.pid), "--nonce", "c").returncode, 0)
        parent.wait(timeout=2)
        r = self.run_guard("gate", "--task-id", "root")
        self.assertEqual(r.returncode, 3)
        blockers = json.loads(r.stdout)["blockers"]
        self.assertEqual([b["task_id"] for b in blockers], ["child"])


if __name__ == "__main__":
    unittest.main()
