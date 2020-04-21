@echo off

rm -rf _site/

echo Generating
docfx

echo Deploying
ssh docs.tgrhavoc.co.uk "mkdir -p /sharedfolders/websites/docs.tgrhavoc.co.uk/livemap-resource/"
scp -r _site/* docs.tgrhavoc.co.uk:/sharedfolders/websites/docs.tgrhavoc.co.uk/livemap-resource/

echo Done!
