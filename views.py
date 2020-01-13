from django.shortcuts import render

from . import varDict
from .forms import PostForm


def layout(request):
    return render(request, 'krog/layout.html', varDict)


def post_new(request):
    form = PostForm()
    return render(request, 'krog/personLayout.html', {'form': form})
