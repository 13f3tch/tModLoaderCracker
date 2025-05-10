using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class TModLoaderPatcher
{
    static void Main()
    {
        const string inputPath = "tModLoader.dll";
        const string outputPath = "tModLoader_patched.dll";

        try
        {
            var assembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters
            {
                ReadWrite = false,
                InMemory = true
            });

            var module = assembly.MainModule;

            var type = module.Types.FirstOrDefault(t => t.FullName == "Terraria.ModLoader.Engine.InstallVerifier");
            if (type == null)
            {
                Console.WriteLine("ERROR: InstallVerifier type not found");
                return;
            }

            var method = type.Methods.FirstOrDefault(m => m.Name == "CheckGoG");
            if (method == null)
            {
                Console.WriteLine("ERROR: CheckGoG method not found");
                return;
            }

            Console.WriteLine("Patching CheckGoG method...");

            var il = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions.ToList();

            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].OpCode == OpCodes.Ldsfld && 
                    instructions[i].Operand?.ToString()?.Contains("vanillaExePath") == true)
                {
                    for (int j = i; j < instructions.Count; j++)
                    {
                        if (instructions[j].OpCode == OpCodes.Call && 
                            instructions[j].Operand is MethodReference methodRef &&
                            methodRef.FullName?.Contains("ErrorReporting::FatalExit") == true)
                        {
                            startIndex = i;
                            endIndex = j;
                            break;
                        }
                    }
                    if (startIndex != -1) break;
                }
            }

            if (startIndex == -1 || endIndex == -1)
            {
                Console.WriteLine("ERROR: Could not locate verification block");
                return;
            }

            Console.WriteLine($"Removing instructions {startIndex} to {endIndex}");

            for (int i = endIndex; i >= startIndex; i--)
            {
                il.Remove(instructions[i]);
            }

            assembly.Write(outputPath);
            Console.WriteLine($"Successfully patched: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}