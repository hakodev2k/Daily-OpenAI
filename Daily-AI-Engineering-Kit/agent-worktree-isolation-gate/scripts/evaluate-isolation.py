#!/usr/bin/env python3
import argparse, fnmatch, hashlib, json, sys
from pathlib import Path


def canonical(v): return json.dumps(v,sort_keys=True,separators=(',',':'))
def match_any(path, patterns): return any(fnmatch.fnmatch(path,p) for p in patterns)

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--session',required=True); ap.add_argument('--state',required=True)
    ap.add_argument('--policy',required=True); ap.add_argument('--changed-paths',required=True)
    ap.add_argument('--active-sessions'); ap.add_argument('--output',required=True)
    ns=ap.parse_args()
    try:
        session=json.load(open(ns.session,encoding='utf-8')); state=json.load(open(ns.state,encoding='utf-8'))
        policy=json.load(open(ns.policy,encoding='utf-8'))
        changed=[x.strip().replace('\\','/') for x in Path(ns.changed_paths).read_text(encoding='utf-8').splitlines() if x.strip()]
        blockers=[]; warnings=[]; collisions=[]
        if policy.get('require_dedicated_branch') and state['branch']!=session['branch']: blockers.append('branch-mismatch')
        if policy.get('require_dedicated_worktree') and str(Path(state['worktree_path']).resolve())!=str(Path(session['worktree_path']).resolve()): blockers.append('worktree-path-mismatch')
        if policy.get('require_clean_start') and session.get('dirty_at_start',False): blockers.append('dirty-start')
        allowed=session['allowed_paths']
        for p in changed:
            if not match_any(p,allowed): blockers.append('out-of-scope:'+p)
            if match_any(p,policy.get('high_risk_paths',[])): warnings.append('high-risk-path:'+p)
        if ns.active_sessions and Path(ns.active_sessions).exists():
            active=json.load(open(ns.active_sessions,encoding='utf-8'))
            for other in active:
                if other.get('session_id')==session['session_id']: continue
                if policy.get('forbid_shared_branch') and other.get('branch')==session['branch']: collisions.append('shared-branch:'+other.get('session_id','unknown'))
                if policy.get('forbid_shared_worktree_path') and other.get('worktree_path')==session['worktree_path']: collisions.append('shared-worktree:'+other.get('session_id','unknown'))
                overlap=set(changed)&set(other.get('changed_paths',[]))
                collisions += ['path-collision:'+p for p in sorted(overlap)]
        blockers += collisions
        status='blocked' if blockers else ('review-required' if warnings or session.get('risk') in ('high','critical') else 'pass')
        report={'version':'1.0','session_id':session['session_id'],'status':status,'branch':state['branch'],'worktree_path':state['worktree_path'],'head_revision':state['head_revision'],'changed_paths':changed,'collisions':collisions,'blockers':sorted(set(blockers)),'warnings':sorted(set(warnings))}
        report['fingerprint']=hashlib.sha256(canonical(report).encode()).hexdigest()
        Path(ns.output).write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
        print(json.dumps({'status':status,'fingerprint':report['fingerprint']}))
        return 2 if status=='blocked' else (3 if status=='review-required' else 0)
    except Exception as e:
        print(json.dumps({'status':'error','error':str(e)})); return 1
if __name__=='__main__': sys.exit(main())
