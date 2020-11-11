FROM microsoft/dotnet:2.1-aspnetcore-runtime
COPY /root /root
ENTRYPOINT ["chmod","777","/root/startup.bash"]
ENTRYPOINT ["sh","/root/startup.bash"]