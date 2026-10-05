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
            Vector3 eye=head.position+station.TransformVector(new Vector3(0,.045f,.18f));
            Pose(0,eye,eye+station.forward*2.0f-station.up*1.8f,94);
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
            var head=arena.Characters[arena.HumanIndex].GetComponent<ImportedMotionPilot>().Rig.GetBoneTransform(HumanBodyBones.Head);
            float pull=Mathf.SmoothStep(0,1,Mathf.InverseLerp(8,29,seconds));
            var close=head.position+s.TransformVector(new Vector3(.34f,-.14f,.72f));
            var wide=s.TransformPoint(new Vector3(.85f,1.60f,.05f));
            var target=Vector3.Lerp(head.position+s.up*.045f,s.TransformPoint(new Vector3(0,1.38f,-.80f)),pull);
            Pose(3,Vector3.Lerp(close,wide,pull),target,Mathf.Lerp(31,42,pull));Use(3);
        }
        public void SnapIntro(){Intro(0);arena.Camera.transform.SetPositionAndRotation(roles[3].transform.position,roles[3].transform.rotation);arena.Camera.fieldOfView=roles[3].fieldOfView;}
        public void Event(int index,bool human)
        {
            var s=arena.Stations[index];int r=human?3:2;
            Pose(r,s.TransformPoint(new Vector3(1.1f,1.9f,.95f)),s.TransformPoint(new Vector3(0,1.03f,-.82f)),48);Use(r);
        }
        public void Results(int winner,bool final)
        {
            if(final){var station=arena.Stations[winner];var position=station.TransformPoint(new Vector3(2.9f,1.9f,0));var target=station.TransformPoint(new Vector3(0,1.40f,-1));target-=Quaternion.LookRotation(target-position)*Vector3.right*.95f;Pose(5,position,target,48);Use(5);return;}
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
