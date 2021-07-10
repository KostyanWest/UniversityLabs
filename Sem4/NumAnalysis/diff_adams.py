from matplotlib import pylab
from scipy.integrate import odeint as od
import numpy as np

EPS = 0.001

a = 0
b = 1
y0 = 0
epsilon = 0.001

def to_fixed(numObj, digits=0):
    if isinstance(numObj, list):
        return [to_fixed(i, digits) for i in numObj]
    return f"{numObj:.{digits}f}"


def f(y, x):
    return (0.7*(1-y**2))/((1+2.0)*(x**2)+(y**2)+1)

def exact_points(step, xlist):
    exact_points = []   
    for x in xlist:
        r2 = exact(x)
        exact_points.append(r2)
    return exact_points

def adams_r(step):
    y1 = y0
    xlist = np.arange(a, b + step, step)
    exact_point = exact_points(step, xlist)
    adams_points = []
    adams_points.append(y1)
    adams_points.append(exact_point[1])
    for i in range(1, len(xlist)-1, 1):
        y1 = adams(xlist[i], adams_points[i], xlist[i-1], adams_points[i-1], step)
        adams_points.append(y1)
    sum = 0
    for i in adams_points:
        sum += i
    return i
    
def find_step():
    h0 = epsilon ** (1/4)
    n = int((b - a) // h0)
    if n % 2 != 0 :
        n += 1
    while check_step(n, epsilon):
        n = n // 4 * 2
    while not check_step(n, epsilon):
        n *= 2
    return (b - a) / n

def check_step(n, epsilon):
    h = (b - a) / n     
    y2 = adams_r(h)
    y2e = adams_r(h * 2)
    eps = (1/15) * abs(y2 - y2e)
    return eps < epsilon

def adams(x1, y1, x2, y2, step):
    return (y1 + step * ((3/2)*f(y1, x1)-(1/2)*f(y2, x2)))
      
def exact(x):
    sol = od(f, y0, [a, x])
    return sol[1][0]  

    
def start_test(variant):
    print("Исходные данные:")
    print(f"y = (0.7*(1-(y**2)))/((1+2.0)*(x**2)+(y**2)+1)")
    print(f"y(0) = {y0}")
    print(f"\nИнтервал: [{a}, {b}]")
    print(f"Погрешность: {EPS}")
    print()
    step = find_step()
    print("Шаг итерирования: ", round(step, 5))
    xlist = np.arange(a, b + step, step)
    adams_points = []
    exact_point = exact_points(step, xlist) 
    y1 = y0
    adams_points.append(y1)
    adams_points.append(exact_point[1])
    for i in range(1, len(xlist)-1, 1):
        y1 = adams(xlist[i], adams_points[i], xlist[i-1], adams_points[i-1], step)
        adams_points.append(y1)
        
    print(f"Значения функции в точках методом Адамса:\n {to_fixed(adams_points, 3)}")
    print(f"Точные значения функции:\n {to_fixed(exact_point, 3)}")    
    pylab.cla()
    pylab.plot (xlist, exact_point, label = "точное решение", color = (0, 1, 0))
    pylab.plot (xlist, adams_points, label = "кривая методом Адамса", color = (1, 0, 0))
    pylab.grid(True)
    pylab.legend()
    pylab.savefig("lab9.png")
    pylab.show()

if __name__=="__main__":
    try:
        start_test(12)
    except KeyboardInterrupt:
        print("Program interrupted by user.")
    else:
        print("\tTest is done.")
