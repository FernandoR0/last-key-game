import bpy,sys,json,re,math
from pathlib import Path
from mathutils import Vector
if bpy.context.object and bpy.context.object.mode!='OBJECT':bpy.ops.object.mode_set(mode='OBJECT')
kind=sys.argv[sys.argv.index('--')+1];out=Path(sys.argv[-1]);out.mkdir(parents=True,exist_ok=True)
rig=next(o for o in bpy.data.objects if o.type=='ARMATURE' and o.name.lower().startswith('rig'))
# Reuse original Blender Studio geometry, clothes, weights and expression controls.
geo=[o for o in bpy.data.objects if o.type=='MESH' and o.name.startswith('GEO-') and not o.hide_render and not any(x in o.name for x in ['cornea','eye_dots','helper','deformer','scarf','ponytail','hair_strand','hairband'])]
for o in geo:
 o.hide_set(False)
 for m in list(o.modifiers):
  if m.type=='SOLIDIFY':o.modifiers.remove(m);continue
  if m.type=='SUBSURF':m.levels=1 if o.name.endswith(('-head','-body')) else 0;m.render_levels=m.levels
bpy.context.view_layer.update();deps=bpy.context.evaluated_depsgraph_get()
base={'Hips':'DEF-Pelvis' if kind=='rain' else 'DEF-Hips','Spine':'DEF-Spine1' if kind=='rain' else 'DEF-Spine','Chest':'DEF-Spine2' if kind=='rain' else 'DEF-RibCage','UpperChest':'DEF-Spine3' if kind=='rain' else 'DEF-Chest','Neck':'DEF-Neck','Head':'DEF-Head'}
parents={'Hips':None,'Spine':'Hips','Chest':'Spine','UpperChest':'Chest','Neck':'UpperChest','Head':'Neck'}
for side,suffix in [('Left','.L'),('Right','.R')]:
 for dst,src in [('Shoulder','DEF-Clavicle' if kind=='rain' else 'DEF-Shoulder'),('UpperArm','DEF-Upperarm1' if kind=='rain' else 'DEF-UpperArm_1'),('LowerArm','DEF-Forearm1' if kind=='rain' else 'DEF-Forearm_1'),('Hand','DEF-Hand' if kind=='rain' else 'DEF-Wrist'),('UpperLeg','DEF-Thigh1' if kind=='rain' else 'DEF-Thigh_1'),('LowerLeg','DEF-Shin1' if kind=='rain' else 'DEF-Knee_1'),('Foot','DEF-Foot'),('Toes','DEF-Toe' if kind=='rain' else 'DEF-Toe')]:
  source=src+suffix
  if source not in rig.data.bones and dst=='Toes':continue
  base[side+dst]=source
 chain=['Shoulder','UpperArm','LowerArm','Hand']
 for i,x in enumerate(chain):parents[side+x]='UpperChest' if i==0 else side+chain[i-1]
 chain=['UpperLeg','LowerLeg','Foot','Toes']
 for i,x in enumerate(chain):
  if side+x in base:parents[side+x]='Hips' if i==0 else side+chain[i-1]
 for finger in ['Thumb','Index','Middle','Ring','Pinky']:
  for n,part in [(1,'Proximal'),(2,'Intermediate'),(3,'Distal')]:
   source=('DEF-' if kind=='rain' else 'DEF-Finger_')+finger+str(n)+suffix
   target=side+('Little' if finger=='Pinky' else finger)+part
   if source in rig.data.bones:base[target]=source;parents[target]=side+'Hand' if n==1 else side+('Little' if finger=='Pinky' else finger)+('Proximal' if n==2 else 'Intermediate')
# Evaluate neutral meshes with masking, mirror and original skin deformation intact.
exports=[]
for source in geo:
 mesh=bpy.data.meshes.new_from_object(source.evaluated_get(deps),preserve_all_data_layers=True,depsgraph=deps)
 ob=bpy.data.objects.new(source.name.replace('GEO-',kind+'_'),mesh);bpy.context.collection.objects.link(ob);ob.matrix_world=source.matrix_world
 exports.append((source,ob))
# Preserve the source skin mask; extra proximity deletion cut visible neck/shoulder faces.
# Bake the source rig's face controls as game-ready morphs; no hand-modelled face mesh.
controls={p.name:p.matrix_basis.copy() for p in rig.pose.bones}
def reset():
 for n,v in controls.items():rig.pose.bones[n].matrix_basis=v
 bpy.context.view_layer.update()
expressions=['Blink','Alert','Pain','Smile','GazeLeft','GazeRight']
for expr in expressions:
 reset()
 for side in ['.L','.R']:
  if expr=='Blink':
   n=('MSTR-Eyering_Upper' if kind=='rain' else 'MSTR-Eyelid_Upper')+side
   if n in rig.pose.bones:rig.pose.bones[n].location.y=-.035
  elif expr in ['Alert','Pain']:
   n='MSTR-Eyebrow'+side
   if n in rig.pose.bones:rig.pose.bones[n].location.y=.013 if expr=='Alert' else -.009
   n='MSTR-Jaw' if kind=='rain' else 'Jaw'
   if n in rig.pose.bones:rig.pose.bones[n].rotation_mode='XYZ';rig.pose.bones[n].rotation_euler.x=.15 if expr=='Alert' else .32
  elif expr=='Smile':
   n='ACT-Lips_Corner'+side
   if n in rig.pose.bones:rig.pose.bones[n].location.x=.009;rig.pose.bones[n].location.y=.012
  elif expr.startswith('Gaze'):
   n='TGT-Eye'+side
   if n in rig.pose.bones:rig.pose.bones[n].location.x=.04 if expr=='GazeLeft' else -.04
 bpy.context.view_layer.update();deps=bpy.context.evaluated_depsgraph_get()
 for source,ob in exports:
  if any(x in source.name for x in ['head','eye','gums','tongue','teeth']):
   em=bpy.data.meshes.new_from_object(source.evaluated_get(deps),preserve_all_data_layers=True,depsgraph=deps)
   if len(em.vertices)==len(ob.data.vertices):
    if ob.data.shape_keys is None:ob.shape_key_add(name='Basis')
    key=ob.shape_key_add(name=expr)
    for i,v in enumerate(em.vertices):key.data[i].co=v.co
   bpy.data.meshes.remove(em)
reset()
# Build a lightweight export skeleton from the original body landmarks.
arm=bpy.data.armatures.new(kind+'_Humanoid');newrig=bpy.data.objects.new(kind+'_Humanoid',arm);bpy.context.collection.objects.link(newrig);newrig.matrix_world=rig.matrix_world
bpy.ops.object.select_all(action='DESELECT');newrig.select_set(True);bpy.context.view_layer.objects.active=newrig;bpy.ops.object.mode_set(mode='EDIT')
for dst,src in base.items():
 b=arm.edit_bones.new(dst);p=rig.pose.bones[src];b.head=p.head;b.tail=p.tail
 if (b.tail-b.head).length<.005:b.tail=b.head+Vector((0,0,.03))
for dst,parent in parents.items():
 if parent and dst in arm.edit_bones and parent in arm.edit_bones:arm.edit_bones[dst].parent=arm.edit_bones[parent]
# Long split limbs become conventional two-bone chains.
for side in ['Left','Right']:
 for a,b in [('UpperArm','LowerArm'),('LowerArm','Hand'),('UpperLeg','LowerLeg'),('LowerLeg','Foot')]:arm.edit_bones[side+a].tail=arm.edit_bones[side+b].head
bpy.ops.object.mode_set(mode='OBJECT')
def map_group(name):
 for target,source in base.items():
  if name==source:return target
 side='Left' if name.endswith('.L') else 'Right' if name.endswith('.R') else ''
 lo=name.lower()
 if side:
  if 'thigh' in lo:return side+'UpperLeg'
  if any(x in lo for x in ['shin','knee']):return side+'LowerLeg'
  if 'upperarm' in lo:return side+'UpperArm'
  if 'forearm' in lo:return side+'LowerArm'
  if 'clavicle' in lo or 'shoulder' in lo:return side+'Shoulder'
  if 'toe' in lo:return side+'Toes' if side+'Toes' in base else side+'Foot'
  if 'ankle' in lo or 'foot' in lo or 'shoe' in lo:return side+'Foot'
  if 'finger' in lo or any(x in lo for x in ['index','middle','ring','pinky','thumb']):
   for f in ['Thumb','Index','Middle','Ring','Pinky']:
    if f.lower() in lo:
     num=re.search(f+r'[^0-9]*([123])',name,re.I);n=int(num.group(1)) if num else 1
     target=side+('Little' if f=='Pinky' else f)+['Proximal','Intermediate','Distal'][n-1]
     if target in base:return target
   return side+'Hand'
  if 'wrist' in lo or 'hand' in lo:return side+'Hand'
 if any(x in lo for x in ['pelvis','hips','hip_']):return 'Hips'
 if 'spine1' in lo or name=='DEF-Spine':return 'Spine'
 if 'spine2' in lo or 'ribcage' in lo:return 'Chest'
 if 'spine3' in lo or 'chest' in lo:return 'UpperChest'
 if 'neck' in lo:return 'Neck'
 if name in rig.pose.bones:
  pos=rig.pose.bones[name].head
  return min(base,key=lambda n:(rig.pose.bones[base[n]].head-pos).length_squared)
 return 'Hips'
for source,ob in exports:
 weights=[];names=[g.name for g in source.vertex_groups]
 face=any(x in source.name for x in ['head','eye','gums','tongue','teeth','hair'])
 for v in ob.data.vertices:
  w={}
  for g in v.groups:
   if g.group<len(names) and names[g.group] in rig.data.bones and rig.data.bones[names[g.group]].use_deform:
    group=names[g.group];lower=group.lower()
    # The imported head includes neck/upper-chest vertices. Preserve their source
    # deformation weights so the neck seam follows the torso, not the face.
    structural=any(k in lower for k in ['neck','spine','ribcage','chest','clavicle'])
    n='Head' if face and not structural else map_group(group);w[n]=w.get(n,0)+g.weight
  if not w:w={'Head' if face else 'Hips':1}
  weights.append(w)
 ob.vertex_groups.clear();groups={n:ob.vertex_groups.new(name=n) for n in base}
 for i,w in enumerate(weights):
  strongest=sorted(w.items(),key=lambda x:x[1],reverse=True)[:4];total=sum(x[1] for x in strongest)
  for n,v in strongest:groups[n].add([i],v/max(total,.00001),'REPLACE')
 m=ob.modifiers.new('Humanoid skin','ARMATURE');m.object=newrig;ob.parent=newrig
 # Export straightforward material colors; original garment geometry is retained.
 for mat in ob.data.materials:
  if mat:mat.use_nodes=True
bpy.ops.object.select_all(action='DESELECT');newrig.select_set(True)
for _,o in exports:o.select_set(True)
bpy.context.view_layer.objects.active=newrig
bpy.ops.export_scene.fbx(filepath=str(out/(kind+'_unity.fbx')),use_selection=True,object_types={'ARMATURE','MESH'},add_leaf_bones=False,bake_anim=False,use_armature_deform_only=True,use_mesh_modifiers=False,axis_forward='-Z',axis_up='Y',path_mode='COPY',embed_textures=False)
report={'kind':kind,'bones':list(base),'meshes':[{'name':o.name,'vertices':len(o.data.vertices),'morphs':{k.name:max((k.data[i].co-v.co).length for i,v in enumerate(o.data.vertices)) for k in o.data.shape_keys.key_blocks} if o.data.shape_keys else {}} for _,o in exports]}
(out/(kind+'_export.json')).write_text(json.dumps(report,indent=2),encoding='utf-8');print('EXPORT_OK',kind,len(base),sum(len(o.data.vertices) for _,o in exports))

