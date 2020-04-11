/*
Lang	C
Sem		2
Lab		5
Task	5.1 (13)
*/

#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <errno.h>

#include "Sem2Lab5_queue.h"
#include "Sem2Lab5_EPN.h"
#include "Sem2Lab5_EMNIA.h"


void ReverseQueue(Queue* pCurQueue, void (*CurEnqueue) (Queue*, int), int (*CurDequeue) (Queue*))
{
	typedef struct _TempStruct
	{
		int value;
		struct _TempStruct* pPrev;
	} TempStruct;
	TempStruct* pLast = NULL;

	/* Dequeue */
	errno = 0;
	while (1) {
		TempStruct* pTemp;
		int num;

		num = CurDequeue(pCurQueue);
		if (errno) {
			break;
		}

		pTemp = malloc(sizeof(TempStruct));
		if (!pTemp) {
			errno = ENOMEM;
			return;
		}
		pTemp->value = num;
		if (pLast) {
			pTemp->pPrev = pLast;
		} else {
			pTemp->pPrev = NULL;
		}
		pLast = pTemp;
	}

	/* Enqueue */
	errno = 0;
	while (pLast) {
		TempStruct* pTemp = pLast;
		CurEnqueue(pCurQueue, pTemp->value);
		if (errno) {
			return;
		}
		pLast = pTemp->pPrev;
		free(pTemp);
	}
}

int main(void)
{
	int count;
	int* pArr;
	Queue queue;
	
	/* Enter size and numbers of queue */
	printf("-> Enter positive size of queue:\n");
	EnterPositiveNum(&count);
	pArr = malloc(count * sizeof(int));
	if (!pArr) {
		printf("Error: out of memory!\n");
		_getch();
		return -1;
	}
	printf("-> Enter numbers of queue:\n");
	EnterMultiNumInArray(count, pArr);

	/* Filling queue*/
	queue.pFirst = NULL;
	queue.pLast = NULL;
	errno = 0;
	for (int i = 0; i < count; i++) {
		Enqueue(&queue, pArr[i]);
		if (errno) {
			FreeQueue(&queue);
			free(pArr);
			printf("Error: out of memory!\n");
			_getch();
			return -1;
		}
	}
	free(pArr);

	/* Reverse queue */
	ReverseQueue(&queue, Enqueue, Dequeue);
	if (errno) {
		FreeQueue(&queue);
		printf("Error: out of memory!\n");
		_getch();
		return -1;
	}

	/* Print reverse queue */
	printf("A reverse queue:\n");
	for (int i = 0; i < count; i++) {
		printf("%d ", Dequeue(&queue));
	}
	printf("\n");

	/* Exit */
	FreeQueue(&queue);
	printf("-> Press any key to exit.\n");
	_getch();
	return 0;
}