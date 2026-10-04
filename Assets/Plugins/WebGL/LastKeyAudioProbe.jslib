mergeInto(LibraryManager.library, {
  LK_AudioReady: function(){return WEBAudio.audioContext&&WEBAudio.audioContext.state==='running'&&WEBAudio.contextIsRunning?1:0;},
  LK_AudioProbe: function() {
    var ctx=WEBAudio.audioContext;
    if(!ctx){document.body.dataset.lkAudioProbe=JSON.stringify({context:'missing',rms:0});return;}
    if(!WEBAudio.lastKeyAnalyser){WEBAudio.lastKeyAnalyser=ctx.createAnalyser();WEBAudio.lastKeyAnalyser.fftSize=1024;WEBAudio.lastKeyProbeGains=new Set();}
    var count=0;
    Object.values(WEBAudio.audioInstances).forEach(function(instance){
      if(instance.gain){count++;try{instance.gain.disconnect(WEBAudio.lastKeyAnalyser);}catch(e){}instance.gain.connect(WEBAudio.lastKeyAnalyser);}
    });
    var samples=new Float32Array(1024);WEBAudio.lastKeyAnalyser.getFloatTimeDomainData(samples);
    var sum=0,peak=0;for(var i=0;i<samples.length;i++){sum+=samples[i]*samples[i];peak=Math.max(peak,Math.abs(samples[i]));}
    document.body.dataset.lkAudioProbe=JSON.stringify({context:ctx.state,sources:Object.values(WEBAudio.audioInstances).filter(x=>x.gain).map(x=>({gain:x.gain.gain.value,stopped:x.isStopped(),hasSource:!!x.source,frames:x.soundClip&&x.soundClip.buffer?x.soundClip.buffer.length:-1,mediaPaused:x.source&&x.source.mediaElement?x.source.mediaElement.paused:null})),rms:Math.sqrt(sum/samples.length),peak:peak,channels:count,sampleRate:ctx.sampleRate,time:ctx.currentTime});
  }
});
