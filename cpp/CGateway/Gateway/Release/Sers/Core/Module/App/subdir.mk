################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../Sers/Core/Module/App/SersApplication.cpp 

OBJS += \
./Sers/Core/Module/App/SersApplication.o 

CPP_DEPS += \
./Sers/Core/Module/App/SersApplication.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/Core/Module/App/%.o: ../Sers/Core/Module/App/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


