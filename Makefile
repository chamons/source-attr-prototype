Q=$(if $(V),,@)

.PHONY: all binding generator restore

BUILD_ARGS=-v:quiet --nologo --no-restore -consoleLoggerParameters:NoSummary
CLEAN_ARGS=-v:quiet --nologo
RESTORE_ARGS=-v:quiet --nologo

run: build
	$(Q) dotnet run --project generator/

build: binding generator
	@:

binding: binding/bin/Debug/net5.0/binding.dll
	@:

generator: generator/bin/Debug/net5.0/generator.dll
	@:

binding/bin/Debug/net5.0/binding.dll: binding/binding.cs binding/binding.csproj binding/PlatformAvailability2.cs attr_converter/EntryPoint.cs attr_converter/attr_converter.csproj
	$(Q) dotnet clean binding/ $(CLEAN_ARGS)
	$(Q) dotnet build binding/ $(BUILD_ARGS)

generator/bin/Debug/net5.0/generator.dll: generator/EntryPoint.cs generator/generator.csproj
	$(Q) dotnet build generator/ $(BUILD_ARGS)

clean:
	$(Q) dotnet clean attr_converter/ $(CLEAN_ARGS)
	$(Q) dotnet clean binding/ $(CLEAN_ARGS)
	$(Q) dotnet clean generator/ $(CLEAN_ARGS)

restore:
	$(Q) dotnet restore attr_converter/ $(RESTORE_ARGS)
	$(Q) dotnet restore binding/ $(RESTORE_ARGS)
	$(Q) dotnet restore generator/ $(RESTORE_ARGS)