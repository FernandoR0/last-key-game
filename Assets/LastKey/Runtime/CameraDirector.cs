using UnityEngine;
namespace LastKey
{
    public sealed class CameraDirector
    {
        readonly ArenaFactory arena;
        readonly Camera[] roles=new Camera[6];
        public string Role {get;private set;}
        public CameraDirector(ArenaFactory value)
        {
            arena=value;var names=new[]{"GameplayPlayerCamera","ArenaWideCamera","RivalEventCamera","EliminationCamera","RoundResultCamera","WinnerCamera"};
            for(int i=0;i<roles.Length;i++){var obj=new GameObject(names[i],typeof(Camera));obj.transform.SetParent(arena.Root.transform);roles[i]=obj.GetComponent<Camera>();roles[i].enabled=false;roles[i].fieldOfView=i==0?58:48;}
        }
        void Pose(int role,Vector3 position,Vector3 target,float fov)
        {
            roles[role].transform.position=position;roles[role].transform.LookAt(target);roles[role].fieldOfView=fov;
        }
        public void Gameplay(bool wide=false)
        {
            var station=arena.Stations[arena.HumanIndex];
            var animator=arena.Characters[arena.HumanIndex].GetComponentInChildren<Animator>();
            var head=animator.GetBoneTransform(HumanBodyBones.Head);
            Vector3 eye=station.TransformPoint(new Vector3(0,1.48f,-.87f));
            Pose(0,eye,eye+station.forward*2.0f-station.up*1.05f,84);
            Pose(1,new Vector3(0,5.4f,-7.8f),new Vector3(0,1,1.0f),54);Use(wide?1:0);
        }
        public void Portrait(int index,bool selection=false)
        {
            var station=arena.Stations[index];
            var position=station.TransformPoint(new Vector3(2.1f,1.6f,-.2f));var target=station.TransformPoint(new Vector3(0,1.1f,-1));
            if(selection)target-=Quaternion.LookRotation(target-position)*Vector3.right*.65f;
            Pose(5,position,target,selection?43:44);Use(5);
        }
        public void Intro(float progress)
        {
            var s=arena.Stations[arena.HumanIndex];float seconds=progress*MusicManager.IntroSeconds;
            if(seconds<4.5f)Pose(3,s.TransformPoint(new Vector3(.52f,1.48f,.10f-Mathf.SmoothStep(0,.18f,seconds/4.5f))),s.TransformPoint(new Vector3(0,1.34f,-.94f)),40);
            else if(seconds<8.5f)Pose(3,s.TransformPoint(new Vector3(.55f,1.28f,-.47f)),s.TransformPoint(new Vector3(0,1.19f,-.94f)),40);
            else if(seconds<11)Pose(3,s.TransformPoint(new Vector3(1.1f,1.05f,-.25f)),s.TransformPoint(new Vector3(0,.60f,-.92f)),48);
            else if(seconds<16)Pose(3,new Vector3(.15f,2.5f,-5.5f),new Vector3(0,1.1f,1.4f),62);
            else {Gameplay();return;}Use(3);
        }
        public void Event(int index,bool human)
        {
            var s=arena.Stations[index];int r=human?3:2;
            Pose(r,s.TransformPoint(new Vector3(1.1f,1.9f,.95f)),s.TransformPoint(new Vector3(0,1.03f,-.82f)),48);Use(r);
        }
        public void Results(int winner,bool final)
        {
            if(final){var station=arena.Stations[winner];Pose(5,station.TransformPoint(new Vector3(2.9f,1.9f,0)),station.TransformPoint(new Vector3(0,1.40f,-1)),48);Use(5);return;}
            Pose(4,new Vector3(0,4.2f,-7.4f),new Vector3(0,1,1),55);Use(4);
        }
        void Use(int role)
        {
            Role=roles[role].name;
            for(int i=0;i<4;i++)foreach(var renderer in arena.Characters[i].GetComponentsInChildren<Renderer>())
            {
                string name=renderer.name.ToLowerInvariant();
                bool face=name.Contains("beard")||name.Contains("hairstyle")||name.Contains("head")||name.Contains("hair")||name.Contains("eye")||name.Contains("gums")||name.Contains("teeth")||name.Contains("tongue");
                renderer.shadowCastingMode=role==0&&i==arena.HumanIndex&&face?UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly:UnityEngine.Rendering.ShadowCastingMode.On;
            }
            float blend=Application.isPlaying&&role<4?1-Mathf.Exp(-Time.deltaTime*7):1;
            arena.Camera.transform.SetPositionAndRotation(Vector3.Lerp(arena.Camera.transform.position,roles[role].transform.position,blend),Quaternion.Slerp(arena.Camera.transform.rotation,roles[role].transform.rotation,blend));arena.Camera.fieldOfView=Mathf.Lerp(arena.Camera.fieldOfView,roles[role].fieldOfView,blend);
        }
    }
}
