# Because of the (frankly weird, in my opinion) default behaviours of Copy-Item, 
# we have to manually create the output folders in case they doesn't exist
New-Item ./wwwroot/css/dist -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
New-Item ./wwwroot/js/dist -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

# CSS Build
Copy-Item ./node_modules/bootstrap/dist/css/bootstrap.min.css ./wwwroot/css/dist
Copy-Item ./node_modules/bootstrap/dist/css/bootstrap.min.css.map ./wwwroot/css/dist
npx cleancss --source-map --output ./wwwroot/css/dist/main.min.css ./wwwroot/css/src/main.css

# JS Build
npx terser ./wwwroot/js/src/main.js --output ./wwwroot/js/dist/main.min.js --source-map --module --ecma 2020
npx terser ./wwwroot/js/src/config.js --output ./wwwroot/js/dist/config.min.js --source-map --module --ecma 2020
