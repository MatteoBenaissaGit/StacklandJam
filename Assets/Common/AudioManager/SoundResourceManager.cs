using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MatteoBenaissaLibrary.SingletonClassBase;
using UnityEditor;
using UnityEngine;

namespace MatteoBenaissaLibrary.AudioManager
{
    public class SoundResourceManager : Singleton<SoundResourceManager>
    {
        [field:Header("Resources")] [field:SerializeField] public List<AudioClip> Resources { get; private set; } = new List<AudioClip>();

        [SerializeField] private string _soundEnumPath = "Assets/MatteoBenaissaLibrary/AudioManager/";
        
        protected override void InternalAwake()
        {
            
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string enumName = "SoundEnum";
            string filePathAndName = _soundEnumPath + enumName + ".cs";
            string[] enumEntries = Resources.Select(x => x.name).ToArray();

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine("	" + enumEntries[i] + ",");
                }

                streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
#endif
        
        public AudioClip GetAudioClip(SoundEnum sound)
        {
            AudioClip clip = Resources[(int)sound];
            if (clip == null)
            {
                throw new Exception("no resource for this");
            }

            return clip;
        }

    }
}