# tModLoaderPatcher
Patch tModLoader for Terraria to remove hash checks to play with a modified .exe (without GoG or Steam). Tested only on Linux, don't know if it will work on Windows

```
cd tModLoader
dotnet new console -n tModLoaderPatcher
cd tModLoaderPatcher
dotnet add package Mono.Cecil
```

After that replace the Program.cs with the Program.cs from this repo and populate the directory with tModLoader dlls:

```
find ../Libraries -type f -name "*.dll" -exec cp {} . \;
cp ../tModLoader.dll .
dotnet run
```

Then you should have a patched tModLoader dll, replace it in the files

```
cp tModLoader_patched.dll ../tModLoader.dll
```

Then just run tModLoader

```
cd ..
sh start-tModLoader.sh
```

(Optional)
If you want you can verify the dlls:

```
cd tModLoaderPatcher
du tModLoader.dll
du tModLoader_patched.dll
avaloniailspy
```

Then in ilspy search for checkGoG method and you'll notice the patcher cut out the hash checker function that was crashing the game if the hash wasn't from steam or gog:
```
private static void CheckGoG()
	{
		if (Path.GetFileName(vanillaExePath) != CheckExe)
		{
			string destFileName = Path.Combine(Path.GetDirectoryName(vanillaExePath), CheckExe);
			Logging.tML.Info("Backing up " + Path.GetFileName(vanillaExePath) + " to " + CheckExe);
			File.Copy(vanillaExePath, destFileName);
		}
	}
```
Could also cut out the checkSteam() but it would result the search menu to not be accessible since you actually need it (I think so at least)

Did not want to download my mods manually and I didn't like tModLoader having a piracy protection so I just patched it

If the patch wouldn't be applied, I'd get this:
```GOG installs must have the unmodified Terraria executable to function. This version of tModLoader expects the Terraria_1.4.1.2.exe version of Terraria, you may need to downgrade or upgrade Terraria via GOG Galaxy, or wait for tModLoader to update.```

Make an issue if they updated something so i'll remake the patcher in my free time
