"""Local-only Renderpeople setup. Do not redistribute extracted/derived assets.
Requires numpy/Pillow. Read ASSET_MANIFEST.md and current Renderpeople terms first.
"""
from pathlib import Path
import urllib.request, zipfile, io
import numpy as np
from PIL import Image
project=Path(__file__).resolve().parents[1]
dest=project/'Assets/PrivateRenderpeople'
cache=project/'Temp/RenderpeopleDownloads'
cache.mkdir(parents=True,exist_ok=True);dest.mkdir(parents=True,exist_ok=True)
chosen=['carla','eric','manuel','nathan']
for kind in ['rigged','animated']:
    archive=cache/(kind+'.zip')
    if not archive.exists():
        url='https://renderpeople.com/sample/free/renderpeople_free_'+kind+'_people_FBX.zip'
        print('Downloading official free package:',url,flush=True)
        urllib.request.urlretrieve(url,archive)
    with zipfile.ZipFile(archive) as outer:
        for name in outer.namelist():
            if not name.endswith('.zip') or not any('rp_'+c+'_' in name for c in chosen):continue
            with zipfile.ZipFile(io.BytesIO(outer.read(name))) as inner:
                for member in inner.infolist():
                    if not (dest/member.filename).resolve().is_relative_to(dest.resolve()):
                        raise ValueError('Unsafe archive path: '+member.filename)
                inner.extractall(dest)
tex=dest/'tex'
for prefix,tint in [('rp_carla_rigged_001',[.24,.46,.48]),('rp_eric_rigged_001',[.28,.36,.52]),('rp_manuel_animated_001',[.35,.41,.29]),('rp_nathan_animated_003',[.46,.26,.28])]:
    image=Image.open(tex/(prefix+'_dif.jpg')).convert('RGB').resize((2048,2048),Image.Resampling.LANCZOS)
    pixels=np.asarray(image).astype(np.float32)
    shirt=np.asarray(Image.open(tex/(prefix+'_mask01.jpg')).convert('L').resize(image.size)).astype(np.float32)/255
    pants=np.asarray(Image.open(tex/(prefix+'_mask02.jpg')).convert('L').resize(image.size)).astype(np.float32)/255
    # Renderpeople mask01 identifies the upper garment; mask02 identifies trousers.
    # Keep skin, hair, shoes and uncovered UV regions untouched.
    other=np.clip(1-shirt-pants,0,1)
    factor=other[:,:,None]+pants[:,:,None]*np.array([.55,.56,.59])+shirt[:,:,None]*np.array(tint)
    Image.fromarray(np.uint8(np.clip(pixels*factor,0,255))).save(tex/(prefix+'_lastkey.png'))
    print(prefix,'existing outfit texture recolored; skin mask preserved')

print("Run LastKey.Editor.RealisticImport.Inspect in Unity, then MusicSetup.Prepare and ProjectBuilder.Prepare.")
