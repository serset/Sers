################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../Sers/Core/Module/json/cJSON/cJSON.c 

OBJS += \
./Sers/Core/Module/json/cJSON/cJSON.o 

C_DEPS += \
./Sers/Core/Module/json/cJSON/cJSON.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/Core/Module/json/cJSON/%.o: ../Sers/Core/Module/json/cJSON/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


