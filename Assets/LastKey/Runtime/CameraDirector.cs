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
            Pose(0,station.TransformPoint(new Vector3(-1.65f,2.9f,-3.25f)),new Vector3(.3f,.45f,.8f),58);
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
            var s=arena.Stations[arena.HumanIndex];float t=progress;
            if(t<.25f)Pose(3,s.TransformPoint(new Vector3(.65f,1.55f,.15f)),s.TransformPoint(new Vector3(0,1.15f,-.95f)),42);
            else if(t<.5f)Pose(3,s.TransformPoint(new Vector3(.9f,1.35f,-.7f)),s.TransformPoint(new Vector3(0,.85f,-1)),46);
            else if(t<.75f)Pose(3,s.TransformPoint(new Vector3(-.65f,1.8f,-1.65f)),s.TransformPoint(new Vector3(0,.97f,-.25f)),48);
            else {Gameplay();return;}Use(3);
        }
        public void Event(int index,bool human)
        {
            var s=arena.Stations[index];int r=human?3:2;
            Pose(r,s.TransformPoint(new Vector3(1.1f,1.9f,.95f)),s.TransformPoint(new Vector3(0,1.03f,-.82f)),48);Use(r);
        }
        public void Results(int winner,bool final)
        {
            if(final){Portrait(winner);var cam=roles[5];cam.transform.LookAt(arena.Stations[winner].TransformPoint(new Vector3(0,1.1f,-1))-cam.transform.right*1.1f);Use(5);return;}
            Pose(4,new Vector3(0,4.2f,-7.4f),new Vector3(0,1,1),55);Use(4);
        }
        void Use(int role)
        {
            Role=roles[role].name;arena.Camera.transform.SetPositionAndRotation(roles[role].transform.position,roles[role].transform.rotation);arena.Camera.fieldOfView=roles[role].fieldOfView;
        }
    }
}
