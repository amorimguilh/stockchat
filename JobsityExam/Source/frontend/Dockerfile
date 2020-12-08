#FROM node:alpine
#EXPOSE 4200
#WORKDIR '/app'
#COPY ./package.json ./
#RUN npm install 
#COPY . . 
#CMD ["npm", "run", "start"]

FROM node:14.8.0-stretch-slim as build-env
WORKDIR /app
COPY ["package.json","package-lock.json","/app/"]
RUN npm install
RUN npm install -g @angular/cli

COPY . /app
#ENTRYPOINT ["ng","--version"]
RUN ng build
CMD ng serve --host 0.0.0.0 --port 4200


#FROM node:12.7-alpine AS build
#WORKDIR /usr/src/app
#COPY package.json package-lock.json ./
#RUN npm install
#COPY . .
#RUN npm run build

# node base image
#FROM nginx:1.17.1-alpine
#COPY ./nginx.conf /etc/nginx/conf.d/nginx.conf
#COPY --from=build /usr/src/app/dist/frontend /usr/share/nginx/html