# Build and minify CSS
npx tailwindcss -i ./wwwroot/css/src/main.css -o ./wwwroot/css/dist/main.css --minify

# Annoyingly, terser won't automatically create the dist folder (there isn't even an option: https://github.com/terser/terser/issues/962)
mkdir ./wwwroot/js/dist -ErrorAction SilentlyContinue | Out-Null
# Minify JS
npx terser ./wwwroot/js/src/main.js --output ./wwwroot/js/dist/main.js --source-map --module --ecma 2020
