# Create output folders
mkdir -p ./wwwroot/css/dist
mkdir -p ./wwwroot/js/dist

# Copy pre-built Bootstrap CSS
cp ./node_modules/bootstrap/dist/css/bootstrap.min.css ./wwwroot/css/dist
cp ./node_modules/bootstrap/dist/css/bootstrap.min.css.map ./wwwroot/css/dist

# Build
npm run css:build
npm run js:build
