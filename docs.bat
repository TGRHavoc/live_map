@echo off

rm -rf _site/
set cwd=%cd%

echo Generating
docfx

echo Deploying
REM ssh docs.tgrhavoc.co.uk "mkdir -p /sharedfolders/websites/docs.tgrhavoc.co.uk/livemap-resource/ && rm -rf /sharedfolders/websites/docs.tgrhavoc.co.uk/livemap-resource/*"
REM scp -r _site/* docs.tgrhavoc.co.uk:/sharedfolders/websites/docs.tgrhavoc.co.uk/livemap-resource/
xcopy /E /I _site %DOC_WEBSITE%\livemap-resource /Y

cd /D %DOC_WEBSITE%
git add livemap-resource\
git commit -m "Update livemap resource documentation"
git push

cd /D %cwd%

echo Done!
