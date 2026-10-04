using System.Collections.Generic;

using System.Linq;

using UnityEngine;

using LastKey.Core;



namespace LastKey

{

    // Downloaded CC0 motion on the imported Humanoid rig, with furniture contact IK:

    // source-rig facial morphs, error reaction and round states. No paid dependencies.

    public sealed class CharacterMotion:MonoBehaviour

    {

        Animator rig;
        ReusableCharacterMotion reused;
        CharacterFace face;
        readonly Quaternion[] neutralFeet=new Quaternion[2];

        Dictionary<Transform,Quaternion> rest;

        Transform station;

        Vector3 basePosition;

        int index,previousErrors;

        float recoil,celebrateBlend;

        Competition game;

        public bool Wakeup,Preview;

        public float IntroProgress,OpeningSeconds;

        public void Initialize(Animator animator,int player,Transform origin)

        {

            rig=animator;index=player;station=origin;basePosition=transform.localPosition;

            rest=rig.GetComponentsInChildren<Transform>().ToDictionary(t=>t,t=>t.localRotation);

            neutralFeet[0]=Quaternion.Inverse(station.rotation)*Bone(HumanBodyBones.LeftFoot).rotation;neutralFeet[1]=Quaternion.Inverse(station.rotation)*Bone(HumanBodyBones.RightFoot).rotation;
            reused=new ReusableCharacterMotion(rig);face=gameObject.AddComponent<CharacterFace>();face.Initialize(index);
            Pose(0);

        }

        public void SetCompetition(Competition value){game=value;}
        void OnDestroy(){reused?.Dispose();}

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

            // Imported animation supplies the body pose; IK only aligns furniture contacts.

            transform.localPosition=basePosition;

            bool eliminated=game!=null&&game.Players[index].Eliminated;

            bool victory=game!=null&&game.Phase==Phase.Finished&&game.Winners().Contains(game.Players[index]);

            bool typing=game!=null&&game.Phase==Phase.Running&&!eliminated&&!Wakeup&&!Preview;

            bool roundWin=game!=null&&game.Phase==Phase.Results&&game.Players[index].RoundScore==game.Players.Max(p=>p.RoundScore);

            bool danger=game!=null&&game.Players[index].Danger>=75&&!eliminated;

            celebrateBlend=Mathf.MoveTowards(celebrateBlend,victory||roundWin?1:0,Time.deltaTime*1.4f);
            bool fast=typing&&game.Players[index].Wpm>=40;

            State=Wakeup?"wakeup":victory?"final_win":eliminated?"eliminated":roundWin?"round_win":recoil>0?"mistake_reaction":danger?"danger_reaction":typing?(fast?"type_fast":"type_normal"):"seated_idle";

            reused.Sample(time,index,game==null?0:game.Players[index].Errors,Wakeup||victory||roundWin,eliminated,typing);
            var hips=Bone(HumanBodyBones.Hips);
            transform.localPosition+=station.InverseTransformVector(station.TransformPoint(new Vector3(0,.66f,-.97f))-hips.position);
            bool human=game==null||index==game.HumanIndex;float wakeStart=human?0:11+index*.4f;float wakeElapsed=OpeningSeconds-wakeStart;
            float awakening=Mathf.Clamp01(wakeElapsed/(human?4.5f:1.4f));
            face.Sample(time,State,Wakeup?awakening:1);
            var spine=Bone(HumanBodyBones.Spine);

            spine.localRotation*=Quaternion.Euler(eliminated?24:2+Mathf.Sin(time*1.7f+index)*.7f,0,recoil>0?Mathf.Sin(time*30)*4:0);

            var head=Bone(HumanBodyBones.Head);head.localRotation*=Quaternion.Euler(Wakeup?Mathf.Lerp(28,2,awakening):victory?-6:eliminated?15:danger?12:2,Mathf.Sin(time*(recoil>0?25:1.1f)+index)*(recoil>0?9:Preview?9:1),0);

            // Base faces -Z; wrapper turns it toward its keyboard.

            for(int side=0;side<2;side++)

            {

                float sign=side==0?-1:1;

                var thigh=Bone(side==0?HumanBodyBones.LeftUpperLeg:HumanBodyBones.RightUpperLeg);

                var calf=Bone(side==0?HumanBodyBones.LeftLowerLeg:HumanBodyBones.RightLowerLeg);

                var foot=Bone(side==0?HumanBodyBones.LeftFoot:HumanBodyBones.RightFoot);

                var ankle=station.TransformPoint(new Vector3(sign*.16f,index==0?.39f:.30f,-.84f));

                TwoBone(thigh,calf,foot,ankle,station.forward);foot.rotation=station.rotation*neutralFeet[side];

                var upper=Bone(side==0?HumanBodyBones.LeftUpperArm:HumanBodyBones.RightUpperArm);

                var lower=Bone(side==0?HumanBodyBones.LeftLowerArm:HumanBodyBones.RightLowerArm);

                var hand=Bone(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand);

                Vector3 target=station.TransformPoint(new Vector3(sign*.18f,.884f+(typing?Mathf.Sin(time*(fast?18:10)+side*3)*.008f:0),-.66f));

                if(eliminated)target=station.TransformPoint(new Vector3(sign*.17f,.7f,-.72f));

                if(celebrateBlend>0)target=Vector3.Lerp(target,head.position+station.TransformDirection(new Vector3(sign*.33f,victory?.18f:.06f,.10f)),Mathf.SmoothStep(0,1,celebrateBlend));

                float reachAt=human?4.5f:1.4f,releaseAt=human?9:3.8f;
                bool collarHand=Wakeup&&side==0&&wakeElapsed>=reachAt&&wakeElapsed<releaseAt;

                if(collarHand){float contact=Mathf.SmoothStep(0,1,Mathf.Clamp01((wakeElapsed-reachAt)/.9f))*Mathf.SmoothStep(0,1,Mathf.Clamp01((releaseAt-wakeElapsed)/.9f));
                    var collar=Bone(HumanBodyBones.Neck).GetComponentInChildren<CollarVisual>();var collarCenter=collar.GetComponentsInChildren<Renderer>()[0].bounds.center;
                    target=Vector3.Lerp(target,collarCenter+station.forward*.07f-station.up*.115f+station.right*sign*.04f,contact);}

                TwoBone(upper,lower,hand,target,station.TransformDirection(new Vector3(sign*.4f,-1,0)));

                // Wrist aligns with the horizontal keyboard instead of retaining a T-pose hand.

                OrientHand(hand,side,station,victory||roundWin||collarHand);

            }

            // Remove gripping poses from the reused idle clip at touchscreen contact.
            // Contact flex adapts the imported fingers without replacing the body motion.
            if(!victory&&!roundWin)foreach(var pair in rest)
            {
                string n=pair.Key.name;
                if(n.Contains("Proximal")||n.Contains("Intermediate")||n.Contains("Distal"))
                {
                    float flex=typing?Mathf.Max(0,Mathf.Sin(time*(fast?15:9)+(n.Contains("Left")?0:2)+(n.Contains("Index")?0:n.Contains("Middle")?1:n.Contains("Ring")?2:3)))*9:3;
                    pair.Key.localRotation=Quaternion.AngleAxis(flex,pair.Key.parent.InverseTransformDirection(station.right))*pair.Value;
                }
            }

        }

        static void OrientHand(Transform hand,int side,Transform station,bool upright)

        {

            var children=hand.GetComponentsInChildren<Transform>();

            var middle=children.First(t=>t.name.Contains("middle_01")||t.name.StartsWith("Middle1.")||t.name.Contains("MiddleProximal"));

            var index=children.First(t=>t.name.Contains("index_01")||t.name.StartsWith("Index1.")||t.name.Contains("IndexProximal"));

            var pinky=children.First(t=>t.name.Contains("pinky_01")||t.name.StartsWith("Pinky1.")||t.name.Contains("LittleProximal"));

            var forward=upright?station.up:station.forward;

            hand.rotation=Quaternion.FromToRotation(middle.position-hand.position,forward)*hand.rotation;

            var across=Vector3.ProjectOnPlane(index.position-pinky.position,forward);

            hand.rotation=Quaternion.AngleAxis(Vector3.SignedAngle(across,side==0?station.right:-station.right,forward),forward)*hand.rotation;

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

