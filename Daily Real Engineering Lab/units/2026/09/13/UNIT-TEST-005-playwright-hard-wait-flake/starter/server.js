const http = require('http');
const page = `<!doctype html><button id="publish">Publish</button><div id="status">Draft</div><script>document.querySelector('#publish').onclick=async()=>{document.querySelector('#status').textContent='Publishing...';const r=await fetch('/publish');document.querySelector('#status').textContent=await r.text();}</script>`;
http.createServer((req,res)=>{
  if(req.url==='/publish') return setTimeout(()=>{res.writeHead(200,{'content-type':'text/plain'});res.end('Published');}, 650);
  res.writeHead(200,{'content-type':'text/html'});res.end(page);
}).listen(4173,'127.0.0.1');
