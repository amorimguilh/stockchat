#FROM node:14.8.0-stretch-slim as build-env
#WORKDIR /app
#COPY ["package.json","package-lock.json","/app/"]
#RUN npm install
#RUN npm install -g @angular/cli

#COPY . /app
#ENTRYPOINT ["ng","--version"]
#RUN ng build
#CMD ng serve --host 0.0.0.0 --port 4200

# Stage 0: compile angular frontend
FROM node:latest as build
WORKDIR /app
COPY . . 
RUN npm install &&\
  npm run build --prod

# Stage 1: serve app with nginx server
FROM nginx:latest
COPY --from=build /app/dist/frontend  /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80