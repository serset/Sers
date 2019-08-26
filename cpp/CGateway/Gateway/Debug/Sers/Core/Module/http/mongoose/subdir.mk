################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../Sers/Core/Module/http/mongoose/HttpServer.cpp \
../Sers/Core/Module/http/mongoose/HttpServerSingleThread.cpp 

C_SRCS += \
../Sers/Core/Module/http/mongoose/mongoose.c 

OBJS += \
./Sers/Core/Module/http/mongoose/HttpServer.o \
./Sers/Core/Module/http/mongoose/HttpServerSingleThread.o \
./Sers/Core/Module/http/mongoose/mongoose.o 

CPP_DEPS += \
./Sers/Core/Module/http/mongoose/HttpServer.d \
./Sers/Core/Module/http/mongoose/HttpServerSingleThread.d 

C_DEPS += \
./Sers/Core/Module/http/mongoose/mongoose.d 


# Each subdirectory must supply rules for building sources it contributes
Sers/Core/Module/http/mongoose/%.o: ../Sers/Core/Module/http/mongoose/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

Sers/Core/Module/http/mongoose/%.o: ../Sers/Core/Module/http/mongoose/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


