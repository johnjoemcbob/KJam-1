 // From: https://answers.unity.com/questions/1095407/saving-webgl.html
 var HandleIO = {
     WindowAlert : function(message)
     {
         window.alert(Pointer_stringify(message));
     },
     SyncFiles : function()
     {
         FS.syncfs(false,function (err) {
             // handle callback
         });
     }
 };
 
 mergeInto(LibraryManager.library, HandleIO);
 
