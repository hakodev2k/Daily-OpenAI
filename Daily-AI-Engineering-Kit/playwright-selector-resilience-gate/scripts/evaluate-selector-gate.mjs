#!/usr/bin/env node
import fs from 'node:fs';

function arg(name,fallback=null){const i=process.argv.indexOf(name);return i>=0&&i+1<process.argv.length?process.argv[i+1]:fallback;}
function die(msg,code=2){console.error(msg);process.exit(code);}
const evaluationPath=arg('--evaluation');
const reviewPath=arg('--review');
const implementationOwner=arg('--implementation-owner','implementation-agent');
const output=arg('--output','selector-gate.json');
if(!evaluationPath||!fs.existsSync(evaluationPath))die('Missing --evaluation');
const e=JSON.parse(fs.readFileSync(evaluationPath,'utf8'));
const reasons=[];
let status='verified';
if(e.status==='blocked'){status='blocked';reasons.push('deterministic-selector-blocker');}
if(status!=='blocked'&&e.status==='review-required'){
  if(!reviewPath||!fs.existsSync(reviewPath)){status='blocked';reasons.push('required-review-missing');}
  else{
    const r=JSON.parse(fs.readFileSync(reviewPath,'utf8'));
    if(r.repository_revision!==e.repository_revision){status='blocked';reasons.push('review-revision-mismatch');}
    if(r.inventory_fingerprint!==e.inventory_fingerprint){status='blocked';reasons.push('review-inventory-fingerprint-mismatch');}
    if(r.reviewer===implementationOwner){status='blocked';reasons.push('self-review-not-allowed');}
    if(r.status!=='approved'){status='blocked';reasons.push(`review-${r.status}`);}
  }
}
const result={status,repository_revision:e.repository_revision,inventory_fingerprint:e.inventory_fingerprint,policy_fingerprint:e.policy_fingerprint,reasons};
fs.writeFileSync(output,JSON.stringify(result,null,2)+'\n');
console.log(JSON.stringify(result,null,2));
process.exit(status==='verified'?0:1);
