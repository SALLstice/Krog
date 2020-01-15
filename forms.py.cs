
using forms = django.forms;

using Post = models.Post;

using System;

public static class forms {
    
    public class PostForm
        : forms.ModelForm {
        
        public class Meta {
            
            public Tuple<string, string> fields;
            
            public object model;
            
            public object model = Post;
            
            public Tuple<string, string> fields = ("title", "text");
        }
    }
}
