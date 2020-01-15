
using path = django.urls.path;

using System.Collections.Generic;

public static class urls {
    
    public static List<object> urlpatterns = new List<object> {
        path("", views.layout, name: "layout"),
        path("people", people.peopleLayout, name: "layout"),
        path("personform", views.post_new, name: "personForm")
    };
}
