
using settings = django.conf.settings;

using models = django.db.models;

using timezone = django.utils.timezone;

public static class models {
    
    public class Post
        : models.Model {
        
        public object author;
        
        public object created_date;
        
        public object published_date;
        
        public object text;
        
        public object title;
        
        public object author = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete: models.CASCADE);
        
        public object title = models.CharField(max_length: 200);
        
        public object text = models.TextField();
        
        public object created_date = models.DateTimeField(@default: timezone.now);
        
        public object published_date = models.DateTimeField(blank: true, null: true);
        
        public virtual void publish() {
            this.published_date = timezone.now();
            this.save();
        }
        
        public override object ToString() {
            return this.title;
        }
    }
}
