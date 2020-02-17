################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../Sers/Gateway/GatewayHelp.cpp 

OBJS += \
./Sers/Gateway/GatewayHelp.o 

CPP_DEPS += \
./Sers/Gateway/GatewayHelp.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/Gateway/%.o: ../Sers/Gateway/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


