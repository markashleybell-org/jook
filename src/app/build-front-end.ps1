# Because of the (frankly weird, in my opinion) default behaviours of Copy-Item, 
# we have to manually create the output folders in case they doesn't exist
New-Item ./wwwroot/css/dist -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
New-Item ./wwwroot/js/dist -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

# Build and minify CSS
Copy-Item ./node_modules/bootstrap/dist/css/bootstrap.min.css ./wwwroot/css/dist
Copy-Item ./node_modules/bootstrap/dist/css/bootstrap.min.css.map ./wwwroot/css/dist

## npx tailwindcss -i ./wwwroot/css/src/main.css -o ./wwwroot/css/dist/main.css --minify

# Annoyingly, terser won't automatically create the dist folder (there isn't even 
# an option to do this: https://github.com/terser/terser/issues/962)
# New-Item ./wwwroot/js/dist/main.min.js -ErrorAction SilentlyContinue | Out-Null

# Minify JS
npx terser ./wwwroot/js/src/main.js --output ./wwwroot/js/dist/main.min.js --source-map --module --ecma 2020
npx terser ./wwwroot/js/src/config.js --output ./wwwroot/js/dist/config.min.js --source-map --module --ecma 2020
