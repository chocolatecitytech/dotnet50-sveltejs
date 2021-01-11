import {createRequest} from './../../utils/api';
export async function post(req, res, next){
  try {
    const {API_ENDPOINT} = process.env;
    const {user} = req.session;
    const request ={
      routePath: API_ENDPOINT + '/accounts/refresh-token',
      options:{
        method: 'POST',
        header: {
          'Content-Type':'application/json',
          Accept: 'application/json'
        }
      },
      token: user.token,
      refreshToken: user.refreshToken
    }
    const data = await createRequest(request);
    if(data){
      req.session.user = data;
      console.log('refreshed session on',data);
      res.setHeader('Content-Type','application/json');
      res.end(JSON.stringify(data));
    }else{
      res.statusCode = 401;
      res.end()
    }

  } catch (error) {
    console.error(error);
  }
}