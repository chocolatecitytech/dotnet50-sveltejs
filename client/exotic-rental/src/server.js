require('dotenv').config();
import sirv from 'sirv';
import polka from 'polka';
import compression from 'compression';
import * as sapper from '@sapper/server';
import bodyParser from 'body-parser';
var session = require('express-session');
var FileStore = require('session-file-store')(session);
const { PORT, NODE_ENV, API_ENDPOINT, SESSION_SECRET } = process.env;
const dev = NODE_ENV === 'development';

polka() 
	.use(bodyParser.json())
	.use(session({
		store: new FileStore(),
    secret: SESSION_SECRET,
    resave: true,
		saveUninitialized: true,
		cookie:{
			maxAge: 3600000
		}
	}))
	.use(
		compression({ threshold: 0 }),
		sirv('static', { dev }),	
		sapper.middleware({
			session: (req) =>{			
				if(req.session && req.session.user){
					console.log('adding user to session on server')
				}
				return {
				API_ENDPOINT,
				user : req.session && req.session.user
				}
			}
		})
	)
	.listen(PORT, err => {
		if (err) console.log('error', err);
	});
