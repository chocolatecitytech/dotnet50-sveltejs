export async function createRequest({routePath,token, refreshToken,options}){
 const isBrowser = process.browser;
 const fetch = isBrowser ? window.fetch : require('node-fetch').default;

 let opts = {};
 Object.assign(opts, options);
 if(!opts.headers){
   opts.headers = {
     Accept:'application/json',
     'Content-Type':'application/json',
   }
 }
 
 if(token){
   opts.headers['Authorization'] = `Bearer ${token}`
 }
 if(refreshToken){
  opts.headers['Refresh-Token-Header'] = refreshToken
 }

 console.log('making request:', routePath)
 console.log('making request:', opts)
   const response = await fetch(routePath, opts)
   if(response.status == 200){
     return await response.json();
   }else if(response.status === 201 || response.status === 204){
     return true;
   }else{
     return null;
   }
}