using b1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace GreatSageMod
{
    public class Utils
    {
        private static UWorld world;

        public static UWorld GetWorld()
        {
            if (world == null)
            {
                UObjectRef uobjectRef = GCHelper.FindRef(FGlobals.GWorld);
                world = uobjectRef?.Managed as UWorld;
            }
            return world;
        }

        public static APawn GetControlledPawn()
        {
            return UGSE_EngineFuncLib.GetFirstLocalPlayerController(GetWorld()).GetControlledPawn();
        }

        public static BGUPlayerCharacterCS GetBGUPlayerCharacterCS()
        {
            return (GetControlledPawn() as BGUPlayerCharacterCS);
        }

        public static BGP_PlayerControllerB1 GetPlayerController()
        {
            return (BGP_PlayerControllerB1)UGSE_EngineFuncLib.GetFirstLocalPlayerController(GetWorld());
        }

        public static BUS_GSEventCollection GetBUS_GSEventCollection()
        {
            return BUS_EventCollectionCS.Get(GetControlledPawn());
        }

        public static T LoadAsset<T>(string asset) where T : UObject
        {
            return b1.BGW.BGW_PreloadAssetMgr.Get(GetWorld()).TryGetCachedResourceObj<T>(asset, b1.BGW.ELoadResourceType.SyncLoadAndCache, b1.BGW.EAssetPriority.Default, null, -1, -1);
        }

        public static UClass LoadClass(string asset)
        {
            return LoadAsset<UClass>(asset);
        }

        public static AActor SpawnActor(string classAsset)
        {
            var controlledPawn = GetControlledPawn();
            FVector actorLocation = controlledPawn.GetActorLocation();
            FVector b = controlledPawn.GetActorForwardVector() * 1000.0f;
            FVector start = actorLocation + b;
            FRotator frotator = UMathLibrary.FindLookAtRotation(start, actorLocation);
            UClass uClass = LoadClass($"PrefabricatorAsset'{classAsset}'");
            if (uClass == null)
            {
                return null;
            }
            return BGUFunctionLibraryCS.BGUSpawnActor(controlledPawn.World, uClass, start, frotator);
        }

        public static AActor GetActorOfClass(string classAsset)
        {
            return UGameplayStatics.GetActorOfClass(GetWorld(), LoadAsset<UClass>(classAsset));
        }

        public static void Log(string msg)
        {
            Console.WriteLine($"[Great Sage Mod] {msg}");
        }
    }
}
