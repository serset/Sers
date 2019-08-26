################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../Sers/ServiceStation/ServiceStation.cpp 

OBJS += \
./Sers/ServiceStation/ServiceStation.o 

CPP_DEPS += \
./Sers/ServiceStation/ServiceStation.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/ServiceStation/%.o: ../Sers/ServiceStation/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


