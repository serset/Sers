
# run on back
echo "start ServiceCenter.bash"
chmod 777 /root/app/ServiceCenter.bash
sh /root/app/ServiceCenter.bash > /root/app/ServiceCenter.log 2>&1 &


# run on back
echo "start Robot.bash"
chmod 777 /root/app/Robot.bash
sh /root/app/Robot.bash > /root/app/Robot.log 2>&1 &


# run on front
echo "start Demo.bash"
chmod 777 /root/app/Demo.bash
sh /root/app/Demo.bash > /root/app/Demo.log 
 





