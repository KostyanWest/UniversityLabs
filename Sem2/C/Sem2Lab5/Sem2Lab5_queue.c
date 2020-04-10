#include <stdio.h>
#include <stdlib.h>
#include <errno.h>

#include "Sem2Lab5_queue.h"

void Enqueue(Queue* pQueue, int element)
{
	QueueElement* pElement = malloc(sizeof(QueueElement));
	if (pElement) {
		pElement->value = element;
		pElement->pPrev = NULL;
	} else {
		errno = ENOMEM;
		return;
	}

	if (pQueue->pFirst) {
		pElement->pNext = pQueue->pFirst;
		pQueue->pFirst->pPrev = pElement;
	} else {
		pElement->pNext = NULL;
		pQueue->pLast = pElement;
	}
	pQueue->pFirst = pElement;
}

int Dequeue(Queue* pQueue)
{
	int result = 0;
	QueueElement* pCurrent = pQueue->pLast;

	if (pCurrent) {
		result = pCurrent->value;
		if (pCurrent->pPrev) {
			pCurrent->pPrev->pNext = NULL;
			pQueue->pLast = pCurrent->pPrev;
			free(pCurrent);
		} else {
			free(pCurrent);
			pQueue->pLast = NULL;
			pQueue->pFirst = NULL;
		}
	} else {
		errno = EAGAIN;
	}
	return result;
}