
using render = django.shortcuts.render;

using PostForm = forms.PostForm;

using System.Collections.Generic;

public static class views {
    
    public static object layout(object request) {
        return render(request, "krog/layout.html", varDict);
    }
    
    public static void post_new(object request) {
        var form = PostForm();
        return render(request, "krog/personLayout.html", new Dictionary<object, object> {
            {
                "form",
                form}});
    }
}
