
echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
call "30.nuget-pack.bat"

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
call "40.Station-publish.bat"
call "40.Station-publish(net5.0).bat"
call "40.Station-publish(net6.0).bat"

call "41.Ñ¹²â-copy.bat"

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
call "50.docker-image-create.bat"
call "51.docker-deploy-copy.bat"

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    


echo %~n0.bat Ö´ÐÐ³É¹¦£¡

pause