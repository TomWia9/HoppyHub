FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build -- --configuration development

FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/hoppy-hub/browser /usr/share/nginx/html
EXPOSE 80