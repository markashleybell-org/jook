# Create output folders
mkdir -p ./wwwroot/css/dist
mkdir -p ./wwwroot/js/dist

# CSS Build
cp ./node_modules/bootstrap/dist/css/bootstrap.min.css ./wwwroot/css/dist
cp ./node_modules/bootstrap/dist/css/bootstrap.min.css.map ./wwwroot/css/dist
npx cleancss --source-map --output ./wwwroot/css/dist/main.min.css ./wwwroot/css/src/main.css

# JS Build
npx terser ./wwwroot/js/src/main.js --output ./wwwroot/js/dist/main.min.js --source-map --module --ecma 2020
npx terser ./wwwroot/js/src/config.js --output ./wwwroot/js/dist/config.min.js --source-map --module --ecma 2020
