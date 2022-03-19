import subprocess
import numpy
from scipy import misc
import matplotlib.pyplot as pyplot


def pip_install(packageName):
    subprocess.call(["pip", "install", packageName])


def installDependencies():
    pip_install("numpy")
    pip_install("scipy")
    pip_install("matplotlib")


def showImage(picture):
    pyplot.imshow(picture)
    pyplot.show()


def main():
    installDependencies()
    picture = misc.face()
    showImage(picture)


main()
