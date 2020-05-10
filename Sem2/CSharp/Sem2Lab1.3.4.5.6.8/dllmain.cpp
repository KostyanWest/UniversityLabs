#include "pch.h"

extern "C" double __declspec(dllexport) __stdcall IntegralLeft (double (*func) (double), double start, double end, long n)
{
	double h = (end - start) / n;
	double sum = 0.0;
	for (double i = start; i < end; i += h) {
		sum += func(i);
	}
	return sum * h;
}

extern "C" double __declspec(dllexport) __stdcall IntegralRight (double (*func) (double), double start, double end, long n)
{
	double h = (end - start) / n;
	double sum = 0.0;
	for (double i = end; i > start; i -= h) {
		sum += func(i);
	}
	return sum * h;
}

extern "C" double __declspec(dllexport) __cdecl IntegralCenter (double (*func) (double), double start, double end, long n)
{
	double h = (end - start) / n;
	double sum = 0.0;
	for (double i = start + h / 2; i < end; i += h) {
		sum += func(i);
	}
	return sum * h;
}

extern "C" double __declspec(dllexport) __cdecl IntegralTrapec (double (*func) (double), double start, double end, long n)
{
	double h = (end - start) / n;
	double sum = 0.0;
	for (double i = start + h; i < end - h; i += h) {
		sum += func(i);
	}
	return (sum + (func(start) + func(end)) / 2.0) * h;
}