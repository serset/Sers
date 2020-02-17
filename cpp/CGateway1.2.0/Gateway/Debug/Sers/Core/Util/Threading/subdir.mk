################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../Sers/Core/Util/Threading/Lock.cpp 

OBJS += \
./Sers/Core/Util/Threading/Lock.o 

CPP_DEPS += \
./Sers/Core/Util/Threading/Lock.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/Core/Util/Threading/%.o: ../Sers/Core/Util/Threading/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


