using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LastKey.Core;

namespace LastKey
{
    // Original procedural animation on the imported CC0 Humanoid rig: seated/idle/typing,
    // error recoil, round elimination slump and seated victory. No paid motion dependency.
    public sealed class CharacterMotion:MonoBehaviour
    {
        Animator rig;
        Dictionary<Transform,Quaternion> rest;
        Transform station;
        Vector3 basePosition;
        int index,previousErrors;
        float recoil;
        Competition game;
        public void Initialize(Animator animator,int player,Transform origin)
        {
            rig=animator;index=player;station=origin;basePosition=transform.localPosition;
            rest=rig.GetComponentsInChildren<Transform>().ToDictionary(t=>t,t=>t.localRotation);
            Pose(0);
        }
        public void SetCompetition(Competition value){game=value;}
        public string State { get; private set; }="SEATED";
        Transform Bone(HumanBodyBones bone){return rig.GetBoneTransform(bone);}
        void LateUpdate()
        {
            if(rest==null)return;
            if(game!=null&&game.Players[index].Errors>previousErrors)recoil=.5f;
            if(game!=null)previousErrors=game.Players[index].Errors;
            recoil=Mathf.Max(0,recoil-Time.deltaTime);Pose(Time.time);
        }
        void Pose(float time)
        {
            foreach(var pair in rest)pair.Key.localRotation=pair.Value;
            transform.localPosition=basePosition;
            bool eliminated=game!=null&&game.Players[index].Eliminated;
            bool victory=game!=null&&game.Phase==Phase.Finished&&game.Winners().Contains(game.Players[index]);
            bool typing=game!=null&&game.Phase==Phase.Running&&!eliminated;
            State=victory?"VICTORY":eliminated?"ELIMINATED":recoil>0?"ERROR":typing?"TYPING":"IDLE SEATED";
            var spine=Bone(HumanBodyBones.Spine);
            spine.localRotation*=Quaternion.Euler(eliminated?24:2+Mathf.Sin(time*1.7f+index)*.7f,0,recoil>0?Mathf.Sin(time*30)*4:0);
            var head=Bone(HumanBodyBones.Head);head.localRotation*=Quaternion.Euler(eliminated?15:2,Mathf.Sin(time*(recoil>0?25:1.1f)+index)*(recoil>0?9:1),0);
            // Base faces -Z; wrapper turns it toward its keyboard.
            for(int side=0;side<2;side++)
            {
                float sign=side==0?-1:1;
                var thigh=Bone(side==0?HumanBodyBones.LeftUpperLeg:HumanBodyBones.RightUpperLeg);
                var calf=Bone(side==0?HumanBodyBones.LeftLowerLeg:HumanBodyBones.RightLowerLeg);
                var foot=Bone(side==0?HumanBodyBones.LeftFoot:HumanBodyBones.RightFoot);
                var ankle=station.TransformPoint(new Vector3(sign*.16f,.12f,-.54f));
                TwoBone(thigh,calf,foot,ankle,Vector3.forward);
                var upper=Bone(side==0?HumanBodyBones.LeftUpperArm:HumanBodyBones.RightUpperArm);
                var lower=Bone(side==0?HumanBodyBones.LeftLowerArm:HumanBodyBones.RightLowerArm);
                var hand=Bone(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand);
                Vector3 target=station.TransformPoint(new Vector3(sign*.18f,.91f+(typing?Mathf.Sin(time*11+side*3)*.008f:0),-.35f));
                if(eliminated)target=station.TransformPoint(new Vector3(sign*.17f,.7f,-.72f));
                if(victory)target=head.position+new Vector3(sign*.46f,.35f,.08f);
                TwoBone(upper,lower,hand,target,new Vector3(sign*.4f,-1,0));
                // Wrist aligns with the horizontal keyboard instead of retaining a T-pose hand.
                OrientHand(hand,side);
            }
            if(typing)foreach(var pair in rest)if(pair.Key.name.Contains("_02_")&&(pair.Key.name.Contains("index")||pair.Key.name.Contains("middle")))pair.Key.localRotation=pair.Value*Quaternion.Euler(Mathf.Sin(time*12+index)*5,0,0);
        }
        static void OrientHand(Transform hand,int side)
        {
            var children=hand.GetComponentsInChildren<Transform>();
            var middle=children.First(t=>t.name.Contains("middle_01"));
            var index=children.First(t=>t.name.Contains("index_01"));
            var pinky=children.First(t=>t.name.Contains("pinky_01"));
            hand.rotation=Quaternion.FromToRotation(middle.position-hand.position,Vector3.forward)*hand.rotation;
            var across=Vector3.ProjectOnPlane(index.position-pinky.position,Vector3.forward);
            hand.rotation=Quaternion.AngleAxis(Vector3.SignedAngle(across,side==0?Vector3.right:Vector3.left,Vector3.forward),Vector3.forward)*hand.rotation;
        }
        static void TwoBone(Transform upper,Transform lower,Transform end,Vector3 target,Vector3 pole)
        {
            Vector3 a=upper.position;float l1=Vector3.Distance(a,lower.position),l2=Vector3.Distance(lower.position,end.position);
            Vector3 direction=target-a;float d=Mathf.Clamp(direction.magnitude,Mathf.Abs(l1-l2)+.001f,l1+l2-.001f);direction.Normalize();
            float along=(l1*l1-l2*l2+d*d)/(2*d);float height=Mathf.Sqrt(Mathf.Max(0,l1*l1-along*along));
            Vector3 bend=Vector3.ProjectOnPlane(pole,direction).normalized;Vector3 elbow=a+direction*along+bend*height;
            upper.rotation=Quaternion.FromToRotation(lower.position-upper.position,elbow-upper.position)*upper.rotation;
            lower.rotation=Quaternion.FromToRotation(end.position-lower.position,target-lower.position)*lower.rotation;
        }
    }
}
