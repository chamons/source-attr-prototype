Q=$(if $(V),,@)

.PHONY: all converter binding generator restore

BUILD_ARGS=-v:quiet --nologo --no-restore
CLEAN_ARGS=-v:quiet --nologo
RESTORE_ARGS=-v:quiet --nologo

run: build
	$(Q) dotnet run --project generator/

build: converter binding generator
	@:

converter: attr_converter/bin/Debug/net5.0/attr_converter.dll
	@:

binding: binding/bin/Debug/net5.0/binding.dll
	@:

generator: generator/bin/Debug/net5.0/generator.dll
	@:

attr_converter/bin/Debug/net5.0/attr_converter.dll: attr_converter/EntryPoint.cs attr_converter/attr_converter.csproj
	$(Q) dotnet build attr_converter/ $(BUILD_ARGS)

binding/bin/Debug/net5.0/binding.dll: binding/binding.cs binding/binding.csproj
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