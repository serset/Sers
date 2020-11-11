
cd   /root/app/CGateway
chmod  -R 777  ./Gateway

while [ 1 -lt 2 ]
do
    echo "run CGateway"
    ./Gateway
    sleep 1
done