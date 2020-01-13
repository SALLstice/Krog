from django.urls import path

from . import people
from . import views

urlpatterns = [
    path('', views.layout, name='layout'),
    path('people', people.peopleLayout, name='layout'),
    path('personform', views.post_new, name='personForm'), ]
