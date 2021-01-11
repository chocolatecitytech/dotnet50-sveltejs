import {createRequest} from './../../utils/api';
export async function post(req, res, next){
  try {
    const {API_ENDPOINT} = process.env;
    const request = {
      routePath: API_ENDPOINT + '/accounts',
      options: {
        method:'POST',
        body: JSON.stringify(req.body),
        credentials: 'include'
      }
    };
    const data = await createRequest(request);
    if(data){
      req.session.user = data;
      res.setHeader('Content-Type','application/json');
      res.end(JSON.stringify(data));
    }else{
      res.statusCode = 401;
      res.end();
    }
  } catch (error) {
    console.error(error);
  }
}