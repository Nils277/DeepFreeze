﻿/**
 * REPOSoftTech KSP Utilities
 * (C) Copyright 2015, Jamie Leighton
 *
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 *
 * Licensed under the Attribution-NonCommercial-ShareAlike (CC BY-NC-SA 4.0) creative commons license. 
 * See <https://creativecommons.org/licenses/by-nc-sa/4.0/> for full details (except where else specified in this file).
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace RSTUtils
{
    public enum GameState
    {
        FLIGHT = 0,
        EDITOR = 1,
        EVA = 2
    }

    internal static class Utilities
    {
        public static int randomSeed = new Random().Next();
        private static int _nextrandomInt = randomSeed;        

        public static int getnextrandomInt()
        {
            _nextrandomInt ++;
            return _nextrandomInt;
        }
		
		//Set the Game State mode indicator, 0 = inflight, 1 = editor, 2 on EVA or F2
        public static bool GameModeisFlight;
        public static bool GameModeisEditor;
        public static bool GameModeisEVA;

        public static GameState SetModeFlag()
        {
            //Set the mode flag, 0 = inflight, 1 = editor, 2 on EVA or F2
            if (FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)  // Check if in flight
            {
                if (FlightGlobals.ActiveVessel.isEVA) // EVA kerbal, do nothing
                {
                    GameModeisEVA = true;
                    GameModeisFlight = GameModeisEditor = false;
                    return GameState.EVA;
                }
                GameModeisFlight = true;
                GameModeisEVA = GameModeisEditor = false;
                return GameState.FLIGHT;
            }
            if (EditorLogic.fetch != null) // Check if in editor
            {
                GameModeisEditor = true;
                GameModeisFlight = GameModeisEVA = false;
                return GameState.EDITOR;
            }
            GameModeisEVA = true;
            GameModeisFlight = GameModeisEditor = false;
            return GameState.EVA;
        }

        //Geometry and space

        public static double DistanceFromHomeWorld(Vessel vessel)
        {
            Vector3d vslPos = vessel.GetWorldPos3D();
            CelestialBody HmePlanet = Planetarium.fetch.Home;
            Log_Debug("Home = " + HmePlanet.name + " Pos = " + HmePlanet.position);
            Log_Debug("Vessel Pos = " + vslPos);
            Vector3d hmeplntPos = HmePlanet.position;
            double DstFrmHome = Math.Sqrt(Math.Pow(vslPos.x - hmeplntPos.x, 2) + Math.Pow(vslPos.y - hmeplntPos.y, 2) + Math.Pow(vslPos.z - hmeplntPos.z, 2));
            Log_Debug("Distance from Home Planet = " + DstFrmHome);
            return DstFrmHome;
        }	
	
		public static void PrintTransform(Transform t, string title = "")
        {
            Log_Debug("------" + title + "------");
            Log_Debug("Position: " + t.localPosition);
            Log_Debug("Rotation: " + t.localRotation);
            Log_Debug("Scale: " + t.localScale);
            Log_Debug("------------------");
        }

        public static void DumpObjectProperties(object o, string title = "---------")
        {
            // Iterate through all of the properties
            Log_Debug("--------- " + title + " ------------");
            foreach (PropertyInfo property in o.GetType().GetProperties())
            {
                if (property.CanRead)
                    Log_Debug(property.Name + " = " + property.GetValue(o, null));
            }
            Log_Debug("--------------------------------------");
        }
		
		// Dump an object by reflection
        internal static void DumpObjectFields(object o, string title = "---------")
        {
            // Dump (by reflection)
            Debug.Log("---------" + title + "------------");
            foreach (FieldInfo field in o.GetType().GetFields())
            {
                if (!field.IsStatic)
                {
                    Debug.Log(field.Name + " = " + field.GetValue(o));
                }
            }
            Debug.Log("--------------------------------------");
        }

        // Use Reflection to get a field from an object
        internal static object GetObjectField(object o, string fieldName)
        {
            object outputObj = new object();
            bool foundObj = false;
            foreach (FieldInfo field in o.GetType().GetFields())
            {
                if (!field.IsStatic)
                {
                    if (field.Name == fieldName)
                    {
                        foundObj = true;
                        outputObj = field.GetValue(o);
                        break;
                    }
                }
            }
            if (foundObj)
            {
                return outputObj;
            }
            return null;
        }

        // Dump all Unity Cameras
        internal static void DumpCameras()
        {
            // Dump (by reflection)
            Debug.Log("--------- Dump Unity Cameras ------------");
            foreach (Camera c in Camera.allCameras)
            {
                Debug.Log("Camera " + c.name + " cullingmask " + c.cullingMask + " depth " + c.depth + " farClipPlane " + c.farClipPlane + " nearClipPlane " + c.nearClipPlane);
            }
            Debug.Log("--------------------------------------");
        }

		public static Camera findCameraByName(string camera)
		{
		    return Camera.allCameras.FirstOrDefault(cam => cam.name == camera);
		}

        /**
          * Recursively searches for a named transform in the Transform heirarchy.  The requirement of
          * such a function is sad.  This should really be in the Unity3D API.  Transform.Find() only
          * searches in the immediate children.
          *
          * @param transform Transform in which is search for named child
          * @param name Name of child to find
          *
          * @return Desired transform or null if it could not be found
          */

        internal static Transform FindInChildren(Transform transform, string name)
        {
            // Is this null?
            if (transform == null)
            {
                return null;
            }

            // Are the names equivalent
            if (transform.name == name)
            {
                return transform;
            }

            // If we did not find a transform, search through the children
            return (from Transform child in transform select FindInChildren(child, name)).FirstOrDefault(t => t != null);

            // Return the transform (will be null if it was not found)
        }
		
		public static Transform FindChildRecursive(Transform parent, string name)
        {
            return parent.gameObject.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == name);
        }

        internal static Camera FindCamera(string name)
        {
            return Camera.allCameras.FirstOrDefault(c => c.name == name);
        }

        internal static void dmpKerbalRefs(Kerbal kerbal, Kerbal seatkerbalref, Kerbal protocrewkerbalref)
        {
            if (kerbal != null)
            {
                Log_Debug("kerbal " + kerbal.name + " " + kerbal.GetInstanceID());
                Log_Debug(kerbal.GetComponent("TRIvaModule") != null
                    ? "kerbal has TRIvaModule attached"
                    : "kerbal DOES NOT have TRIvaModule attached");
            }

            if (seatkerbalref != null)
            {
                Log_Debug("seatkerbalref " + seatkerbalref.name + " " + seatkerbalref.GetInstanceID());
                Log_Debug(seatkerbalref.GetComponent("TRIvaModule") != null
                    ? "seatkerbalref has TRIvaModule attached"
                    : "seatkerbalref DOES NOT have TRIvaModule attached");
            }
            if (protocrewkerbalref != null)
            {
                Log_Debug("protocrewkerbalref " + protocrewkerbalref.name + " " + protocrewkerbalref.GetInstanceID());
                Log_Debug(protocrewkerbalref.GetComponent("TRIvaModule") != null
                    ? "protocrewkerbalref has TRIvaModule attached"
                    : "protocrewkerbalref DOES NOT have TRIvaModule attached");
            }
        }

        internal static void dmpAnimationNames(Animation anim)
        {
            List<AnimationState> states = new List<AnimationState>(anim.Cast<AnimationState>());
            Log_Debug("Animation " + anim.name);
            foreach (AnimationState state in states)
            {
                Log_Debug("Animation clip " + state.name);
            }
        }
		
		public static IEnumerator WaitForAnimation(Animation animation, string name)
        {
            do
            {
                yield return null;
            } while (animation.IsPlaying(name));
        }

        internal static void dmpAllKerbals()
        {
            foreach (Kerbal kerbal in Resources.FindObjectsOfTypeAll<Kerbal>())
            {
                Log_Debug("Kerbal " + kerbal.name + " " + kerbal.crewMemberName + " instance " + kerbal.GetInstanceID() + " rosterstatus " + kerbal.rosterStatus);
                Log_Debug(kerbal.protoCrewMember == null ? "ProtoCrewmember is null " : "ProtoCrewmember exists " + kerbal.protoCrewMember.name);
            }
        }

        // The following method is modified from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        internal static void setTransparentTransforms(this Part thisPart, string transparentTransforms)
        {
            string transparentShaderName = "Transparent/Specular";
            var transparentShader = Shader.Find(transparentShaderName);
            foreach (string transformName in transparentTransforms.Split('|'))
            {
                Log_Debug("setTransparentTransforms " + transformName);
                try
                {
                    Transform tr = thisPart.FindModelTransform(transformName.Trim());
                    if (tr != null)
                    {
                        // We both change the shader and backup the original shader so we can undo it later.
                        Shader backupShader = tr.renderer.material.shader;
                        tr.renderer.material.shader = transparentShader;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Unable to set transparent shader transform " + transformName);
                    Debug.LogException(e);
                }
            }
        }

        // The following method is derived from TextureReplacer mod. Which is licensed as:
        //Copyright © 2013-2015 Davorin Učakar, Ryan Bray
        //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        private static double atmSuitPressure = 50.0;

        internal static bool isAtmBreathable()
        {
            bool value = !HighLogic.LoadedSceneIsFlight
                         || (FlightGlobals.getStaticPressure() >= atmSuitPressure);
            Log_Debug("isATMBreathable Inflight? " + value + " InFlight " + HighLogic.LoadedSceneIsFlight + " StaticPressure " + FlightGlobals.getStaticPressure());
            return value;
        }

        // The following method is derived from TextureReplacer mod. Which is licensed as:
        //Copyright © 2013-2015 Davorin Učakar, Ryan Bray
        //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        private static Mesh[] helmetMesh = { null, null };

        private static Mesh[] visorMesh = { null, null };
        private static bool helmetMeshstored;

        internal static void storeHelmetMesh()
        {
            Log_Debug("StoreHelmetMesh");
            foreach (Kerbal kerbal in Resources.FindObjectsOfTypeAll<Kerbal>())
            {
                int gender = kerbal.transform.name == "kerbalFemale" ? 1 : 0;
                // Save pointer to helmet & visor meshes so helmet removal can restore them.
                foreach (SkinnedMeshRenderer smr in kerbal.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (smr.name.EndsWith("helmet", StringComparison.Ordinal))
                        helmetMesh[gender] = smr.sharedMesh;
                    else if (smr.name.EndsWith("visor", StringComparison.Ordinal))
                        visorMesh[gender] = smr.sharedMesh;
                }
            }
            helmetMeshstored = true;
        }

        // The following method is derived from TextureReplacer mod.Which is licensed as:
        //Copyright © 2013-2015 Davorin Učakar, Ryan Bray
        //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        internal static void setHelmetshaders(Kerbal thatKerbal, bool helmetOn)
        {
            if (!helmetMeshstored)
                storeHelmetMesh();

            //This will check if Atmospher is breathable then we always remove our hetmets regardless.
            if (helmetOn && isAtmBreathable())
            {
                helmetOn = false;
                Log_Debug("setHelmetShaders to put on helmet but in breathable atmosphere");
            }

            try
            {
                foreach (SkinnedMeshRenderer smr in thatKerbal.helmetTransform.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (smr.name.EndsWith("helmet", StringComparison.Ordinal))
                        smr.sharedMesh = helmetOn ? helmetMesh[(int)thatKerbal.protoCrewMember.gender] : null;
                    else if (smr.name.EndsWith("visor", StringComparison.Ordinal))
                        smr.sharedMesh = helmetOn ? visorMesh[(int)thatKerbal.protoCrewMember.gender] : null;
                }
            }
            catch (Exception ex)
            {
                Log("Error attempting to setHelmetshaders for " + thatKerbal.name + " to " + helmetOn);
                Log(ex.Message);
            }
        }

        // The following method is derived from TextureReplacer mod. Which is licensed as:
        //Copyright © 2013-2015 Davorin Učakar, Ryan Bray
        //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        internal static void setHelmets(this Part thisPart, bool helmetOn)
        {
            if (thisPart.internalModel == null)
            {
                Log_Debug("setHelmets but no internalModel");
                return;
            }

            if (!helmetMeshstored)
                storeHelmetMesh();

            Log_Debug("setHelmets helmetOn=" + helmetOn);
            //Kerbal thatKerbal = null;
            foreach (InternalSeat thatSeat in thisPart.internalModel.seats)
            {
                if (thatSeat.crew != null)
                {
                    Kerbal thatKerbal = thatSeat.kerbalRef;
                    if (thatKerbal != null)
                    {
                        thatSeat.allowCrewHelmet = helmetOn;
                        Log_Debug("Setting helmet=" + helmetOn + " for kerbal " + thatSeat.crew.name);
                        // `Kerbal.ShowHelmet(false)` irreversibly removes a helmet while
                        // `Kerbal.ShowHelmet(true)` has no effect at all. We need the following workaround.
                        // I think this can be done using a coroutine to despawn and spawn the internalseat crewmember kerbalref.
                        // But I found this workaround in TextureReplacer so easier to use that.
                        //if (thatKerbal.showHelmet)
                        //{
                        setHelmetshaders(thatKerbal, helmetOn);
                        //}
                        //else
                        //    Log_Debug("Showhelmet is OFF so the helmettransform does not exist");
                    }
                    else
                        Log_Debug("kerbalref = null?");
                }
            }
        }

        // Sets the kerbal layers to make them visible (Thawed) or not (Frozen), setVisible = true sets layers to visible, false turns them off.
        // If bodyOnly is true only the "body01" mesh is changed (to be replaced by placeholder mesh lying down as kerbals in IVA are always in sitting position).
        internal static void setFrznKerbalLayer(ProtoCrewMember kerbal, bool setVisible, bool bodyOnly)
        {
            //Log_Debug("setFrznKerbalLayer " + kerbal.name + " visible " + setVisible);
            int layer = 16;
            if (!setVisible)
            {
                layer = 21;
            }

            foreach (Renderer renderer in kerbal.KerbalRef.GetComponentsInChildren<Renderer>(true))
            {
                if ((bodyOnly && renderer.name == "body01") || !bodyOnly)
                {
                    if (renderer.gameObject.layer == layer)
                    {
                        //Log_Debug("Layers already set");
                        break;
                    }
                    //Log_Debug("Renderer: " + renderer.name + " set to layer " + layer);
                    renderer.gameObject.layer = layer;
                    if (setVisible) renderer.enabled = true;
                    else renderer.enabled = false;
                }
            }
        }

        internal static void CheckPortraitCams(Vessel vessel)
        {
            // Only the pods in the active vessel should be doing it since the list refers to them.
            //Log_Debug("DeepFreeze CheckPortraitCams vessel " + vessel.name + "(" + vessel.id + ") activevessel " + FlightGlobals.ActiveVessel.name + "(" + FlightGlobals.ActiveVessel.id + ")");

            // First, We check through the list of portaits and remove everyone who is from some other vessel, or NO vessel.
            var stowaways = new List<Kerbal>();
            foreach (Kerbal thatKerbal in KerbalGUIManager.ActiveCrew)
            {
                if (thatKerbal.InPart == null)
                {
                    Log_Debug("kerbal " + thatKerbal.name + " Invessel = null add stowaway");
                    stowaways.Add(thatKerbal);
                }
                else
                {
                    Log_Debug("kerbal " + thatKerbal.name + " Invessel = " + thatKerbal.InVessel + " InvesselID = " + thatKerbal.InVessel.id);
                    if (thatKerbal.InVessel.id != FlightGlobals.ActiveVessel.id)
                    {
                        Log_Debug("Adding stowaway");
                        stowaways.Add(thatKerbal);
                    }
                }
            }
            foreach (Kerbal thatKerbal in stowaways)
            {
                KerbalGUIManager.RemoveActiveCrew(thatKerbal);
            }

            if (FlightGlobals.ActiveVessel.id == vessel.id)
            {
                // Then, Check the list of seats in every crewable part in the vessel and see if anyone is missing who should be present.
                List<Part> crewparts = (from p in vessel.parts where p.CrewCapacity > 0 && p.internalModel != null select p).ToList();
                foreach (Part part in crewparts)
                {
                    Log_Debug("Check Portraits for part " + part.name);
                    foreach (InternalSeat seat in part.internalModel.seats)
                    {
                        Log_Debug("checking Seat " + seat.seatTransformName);
                        if (seat.kerbalRef != null) Log_Debug("kerbalref=" + seat.kerbalRef.crewMemberName);
                        else Log_Debug("Seat kerbalref is null");
                        if (seat.kerbalRef != null && !KerbalGUIManager.ActiveCrew.Contains(seat.kerbalRef))
                        {
                            Log_Debug("Checking crewstatus " + seat.kerbalRef.protoCrewMember.rosterStatus + " " + seat.kerbalRef.protoCrewMember.type);
                            if (seat.kerbalRef.protoCrewMember.rosterStatus != ProtoCrewMember.RosterStatus.Dead || seat.kerbalRef.protoCrewMember.type != ProtoCrewMember.KerbalType.Unowned)
                            {
                                Log_Debug("Adding missing Portrait for " + seat.kerbalRef.crewMemberName);
                                KerbalGUIManager.AddActiveCrew(seat.kerbalRef);
                            }
                        }
                    }
                }
            }
            else Log_Debug("Vessel is not active vessel");
        }

        // The following method is taken from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        internal static Kerbal FindCurrentKerbal(this Part thisPart)
        {
            if (thisPart.internalModel == null || !VesselIsInIVA(thisPart.vessel))
                return null;
            // InternalCamera instance does not contain a reference to the kerbal it's looking from.
            // So we have to search through all of them...
            Kerbal thatKerbal = null;
            foreach (InternalSeat thatSeat in thisPart.internalModel.seats)
            {
                if (thatSeat.kerbalRef != null)
                {
                    if (thatSeat.kerbalRef.eyeTransform == InternalCamera.Instance.transform.parent)
                    {
                        thatKerbal = thatSeat.kerbalRef;
                        break;
                    }
                }
            }
            return thatKerbal;
        }

        // The following method is taken from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        internal static bool VesselIsInIVA(Vessel thatVessel)
        {
            // Inactive IVAs are renderer.enabled = false, this can and should be used...
            // ... but now it can't because we're doing transparent pods, so we need a more complicated way to find which pod the player is in.
            return HighLogic.LoadedSceneIsFlight && IsActiveVessel(thatVessel) && IsInIVA();
        }

        // The following method is taken from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        internal static bool IsActiveVessel(Vessel thatVessel)
        {
            return HighLogic.LoadedSceneIsFlight && thatVessel != null && thatVessel.isActiveVessel;
        }

        // The following method is taken from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        internal static bool IsInIVA()
        {
            return CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA;
        }

        internal static bool IsInInternal()
        {
            return CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Internal;
        }
                
        internal static RuntimeAnimatorController kerbalIVAController;

        internal static void subdueIVAKerbalAnimations(Kerbal kerbal)
        {
            try
            {
                foreach (Animator anim in kerbal.gameObject.GetComponentsInChildren<Animator>())
                {
                    if (anim.name == kerbal.name)
                    {
                        kerbalIVAController = anim.runtimeAnimatorController;
                        RuntimeAnimatorController myController = anim.runtimeAnimatorController;
                        AnimatorOverrideController myOverrideController = new AnimatorOverrideController();
                        myOverrideController.runtimeAnimatorController = myController;
                        myOverrideController["idle_animA_upWord"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animB"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animC"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animD_dance"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animE_drummingHelmet"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animI_drummingControls"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animJ_yo"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animJ_IdleLoopShort"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["idle_animK_footStretch"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["head_rotation_staringUp"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["head_rotation_longLookUp"] = myOverrideController["idle_animH_notDoingAnything"];
                        myOverrideController["head_faceExp_fun_ohAh"] = myOverrideController["idle_animH_notDoingAnything"];
                        // Put this line at the end because when you assign a controller on an Animator, unity rebinds all the animated properties
                        anim.runtimeAnimatorController = myOverrideController;
                        Log_Debug("Animator " + anim.name + " for " + kerbal.name + " subdued");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(" failed to subdue IVA animations for " + kerbal.name);
                Debug.LogException(ex);
            }
        }

        internal static void reinvigerateIVAKerbalAnimations(Kerbal kerbal)
        {
            foreach (Animator anim in kerbal.gameObject.GetComponentsInChildren<Animator>())
            {
                if (anim.name == kerbal.name)
                {
                    RuntimeAnimatorController myController = kerbalIVAController;
                    AnimatorOverrideController myOverrideController = new AnimatorOverrideController();
                    myOverrideController.runtimeAnimatorController = myController;
                    // Put this line at the end because when you assign a controller on an Animator, unity rebinds all the animated properties
                    anim.runtimeAnimatorController = myOverrideController;
                    Log_Debug("Animator " + anim.name + " for " + kerbal.name + " reinvigerated");
                }
            }
        }
               

        // The following method is taken from Kerbal Alarm Clock as-is. Which is covered by MIT license.
        internal static int getVesselIdx(Vessel vtarget)
        {
            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                if (FlightGlobals.Vessels[i].id == vtarget.id)
                {
                    Log_Debug("Found Target idx=" + i + " (" + vtarget.id + ")");
                    return i;
                }
            }
            return -1;
        }

        //Temperature
        internal static float KelvintoCelsius(float kelvin)
        {
            return kelvin - 273.15f;
        }

        internal static float CelsiustoKelvin(float celsius)
        {
            return celsius + 273.15f;
        }
		
		//Resources
		public static double GetAvailableResource(Part part, String resourceName)
        {
            var resources = new List<PartResource>();
            part.GetConnectedResources(PartResourceLibrary.Instance.GetDefinition(resourceName).id, ResourceFlowMode.ALL_VESSEL, resources);
            double total = 0;
            foreach (PartResource pr in resources)
            {
                total += pr.amount;
            }
            return total;
        }


        // GUI & Window Methods

        public static int scaledScreenHeight = 1;
        public static int scaledScreenWidth = 1;
        private static bool scaledScreenset;

        internal static void setScaledScreen()
        {
            scaledScreenHeight = Mathf.RoundToInt(Screen.height / 1);
            scaledScreenWidth = Mathf.RoundToInt(Screen.width / 1);
            scaledScreenset = true;
        }

        internal static bool WindowVisibile(Rect winpos)
        {
            if (!scaledScreenset) setScaledScreen();
            float minmargin = 20.0f; // 20 bytes margin for the window
            float xMin = minmargin - winpos.width;
            float xMax = scaledScreenWidth - minmargin;
            float yMin = minmargin - winpos.height;
            float yMax = scaledScreenHeight - minmargin;
            bool xRnge = (winpos.x > xMin) && (winpos.x < xMax);
            bool yRnge = (winpos.y > yMin) && (winpos.y < yMax);
            return xRnge && yRnge;
        }

        internal static Rect MakeWindowVisible(Rect winpos)
        {
            if (!scaledScreenset) setScaledScreen();
            float minmargin = 20.0f; // 20 bytes margin for the window
            float xMin = minmargin - winpos.width;
            float xMax = scaledScreenWidth - minmargin;
            float yMin = minmargin - winpos.height;
            float yMax = scaledScreenHeight - minmargin;

            winpos.x = Mathf.Clamp(winpos.x, xMin, xMax);
            winpos.y = Mathf.Clamp(winpos.y, yMin, yMax);

            return winpos;
        }

        // The following method is taken from RasterPropMonitor as-is. Which is covered by GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
        public static string WordWrap(string text, int maxLineLength)
        {
            var sb = new StringBuilder();
            char[] prc = { ' ', ',', '.', '?', '!', ':', ';', '-' };
            char[] ws = { ' ' };

            foreach (string line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                int currentIndex;
                int lastWrap = 0;
                do
                {
                    currentIndex = lastWrap + maxLineLength > line.Length ? line.Length : line.LastIndexOfAny(prc, Math.Min(line.Length - 1, lastWrap + maxLineLength)) + 1;
                    if (currentIndex <= lastWrap)
                        currentIndex = Math.Min(lastWrap + maxLineLength, line.Length);
                    sb.AppendLine(line.Substring(lastWrap, currentIndex - lastWrap).Trim(ws));
                    lastWrap = currentIndex;
                } while (currentIndex < line.Length);
            }
            return sb.ToString();
        }

        // Get Config Node Values out of a config node Methods

        internal static bool GetNodeValue(ConfigNode confignode, string fieldname, bool defaultValue)
        {
            bool newValue;
            if (confignode.HasValue(fieldname) && bool.TryParse(confignode.GetValue(fieldname), out newValue))
            {
                return newValue;
            }
            return defaultValue;
        }

        internal static int GetNodeValue(ConfigNode confignode, string fieldname, int defaultValue)
        {
            int newValue;
            if (confignode.HasValue(fieldname) && int.TryParse(confignode.GetValue(fieldname), out newValue))
            {
                return newValue;
            }
            return defaultValue;
        }

        internal static float GetNodeValue(ConfigNode confignode, string fieldname, float defaultValue)
        {
            float newValue;
            if (confignode.HasValue(fieldname) && float.TryParse(confignode.GetValue(fieldname), out newValue))
            {
                return newValue;
            }
            return defaultValue;
        }

        internal static double GetNodeValue(ConfigNode confignode, string fieldname, double defaultValue)
        {
            double newValue;
            if (confignode.HasValue(fieldname) && double.TryParse(confignode.GetValue(fieldname), out newValue))
            {
                return newValue;
            }
            return defaultValue;
        }

        internal static string GetNodeValue(ConfigNode confignode, string fieldname, string defaultValue)
        {
            if (confignode.HasValue(fieldname))
            {
                return confignode.GetValue(fieldname);
            }
            return defaultValue;
        }

        internal static Guid GetNodeValue(ConfigNode confignode, string fieldname)
        {
            if (confignode.HasValue(fieldname))
            {
                try
                {
                    Guid id = new Guid(confignode.GetValue(fieldname));
                    return id;
                }
                catch (Exception ex)
                {
                    Debug.Log("Unable to getNodeValue " + fieldname + " from " + confignode);
                    Debug.Log("Err: " + ex);
                    return Guid.Empty;
                }
            }
            return Guid.Empty;
        }

        internal static T GetNodeValue<T>(ConfigNode confignode, string fieldname, T defaultValue) where T : IComparable, IFormattable, IConvertible
        {
            if (confignode.HasValue(fieldname))
            {
                string stringValue = confignode.GetValue(fieldname);
                if (Enum.IsDefined(typeof(T), stringValue))
                {
                    return (T)Enum.Parse(typeof(T), stringValue);
                }
            }
            return defaultValue;
        }
		
		//Formatting time functions
		
		//Format a Time double variable into format "xxxx:year xxxx:days xxxx:hours xxxx:mins x:xx:secs"
        //Future expansion required to format to different formats.
        public static String formatTime(double seconds)
        {
            int y = (int)(seconds / (6.0 * 60.0 * 60.0 * 426.08));
            seconds = seconds % (6.0 * 60.0 * 60.0 * 426.08);
            int d = (int)(seconds / (6.0 * 60.0 * 60.0));
            seconds = seconds % (6.0 * 60.0 * 60.0);
            int h = (int)(seconds / (60.0 * 60.0));
            seconds = seconds % (60.0 * 60.0);
            int m = (int)(seconds / 60.0);
            seconds = seconds % 60.0;

            List<String> parts = new List<String>();

            if (y > 0)
            {
                parts.Add(String.Format("{0}:year ", y));
            }

            if (d > 0)
            {
                parts.Add(String.Format("{0}:days ", d));
            }

            if (h > 0)
            {
                parts.Add(String.Format("{0}:hours ", h));
            }

            if (m > 0)
            {
                parts.Add(String.Format("{0}:mins ", m));
            }

            if (seconds > 0)
            {
                parts.Add(String.Format("{0:00}:secs ", seconds));
            }

            if (parts.Count > 0)
            {
                return String.Join(" ", parts.ToArray());
            }
            return "0s";
        }	

        //Format a Time double variable into format "YxxxxDxxxhh:mm:ss"
        //Future expansion required to format to different formats.
        internal static string FormatDateString(double time)
        {
            string outputstring = string.Empty;
            int[] datestructure = new int[5];
            if (GameSettings.KERBIN_TIME)
            {
                datestructure[0] = (int)time / 60 / 60 / 6 / 426; // Years
                datestructure[1] = (int)time / 60 / 60 / 6 % 426; // Days
                datestructure[2] = (int)time / 60 / 60 % 6;    // Hours
                datestructure[3] = (int)time / 60 % 60;    // Minutes
                datestructure[4] = (int)time % 60; //seconds
            }
            else
            {
                datestructure[0] = (int)time / 60 / 60 / 24 / 365; // Years
                datestructure[1] = (int)time / 60 / 60 / 24 % 365; // Days
                datestructure[2] = (int)time / 60 / 60 % 24;    // Hours
                datestructure[3] = (int)time / 60 % 60;    // Minutes
                datestructure[4] = (int)time % 60; //seconds
            }
            if (datestructure[0] > 0)
                outputstring += "Y" + datestructure[0].ToString("####") + ":";
            if (datestructure[1] > 0)
                outputstring += "D" + datestructure[1].ToString("###") + ":";
            outputstring += datestructure[2].ToString("00:");
            outputstring += datestructure[3].ToString("00:");
            outputstring += datestructure[4].ToString("00");
            return outputstring;
        }

        //Returns True if the PauseMenu is open. Because the GameEvent callbacks don't work on the mainmenu.
        internal static bool isPauseMenuOpen
        {
            get
            {
                try
                {
                    return PauseMenu.isOpen;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Electricity and temperature functions are only valid if timewarp factor is < 5.
        internal static bool timewarpIsValid(int max)
        {
            return TimeWarp.CurrentRateIndex < max;
        }

        internal static void stopWarp()
        {
            TimeWarp.SetRate(0, false);
        }

        // Logging Functions
        // Name of the Assembly that is running this MonoBehaviour
        internal static String _AssemblyName
        { get { return Assembly.GetExecutingAssembly().GetName().Name; } }
		
		internal static bool debuggingOn = false;
		
		/// <summary>
        /// Logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>			
				
		internal static void Log_Debug(String Message, params object[] strParams)
        {
            if (debuggingOn)
			{
				Log("DEBUG: " + Message, strParams);
			}
        }
		
		/// <summary>
        /// Logging to the log file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
               				
		internal static void Log(String Message, params object[] strParams)
        {
            Message = String.Format(Message, strParams);                  // This fills the params into the message
            String strMessageLine = String.Format("{0},{2},{1}",
                DateTime.Now, Message,
                _AssemblyName);                                           // This adds our standardised wrapper to each line
            Debug.Log(strMessageLine);                        // And this puts it in the log
        }
    }
}