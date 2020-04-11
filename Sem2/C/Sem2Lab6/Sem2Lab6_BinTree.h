#ifndef SEM2LAB6_BINTREE_H
#define SEM2LAB6_BINTREE_H

#include <stdio.h>
#include <stdlib.h>
#include <errno.h>

typedef struct _BinTreeElement BinTreeElement;

struct _BinTreeElement
{
	int value;
	BinTreeElement* pLeft;
	BinTreeElement* pRight;
};

BinTreeElement* CreateBinTreeElement (int value);
void DeleteBinTreeElementHierarchy (BinTreeElement* pElement);
char AddChildToBinSearchTree (BinTreeElement* pRoot, BinTreeElement* pElement);

#endif // !SEM2LAB6_BINTREE_H
