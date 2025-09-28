"use client";

import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { ScrollArea, ScrollBar } from "@/components/ui/scroll-area";

export const IngredientsToolbar = () => {
  return (
    <>
      {/* <NewMeetingDialog open={isDialogOpen} onOpenChange={setIsDialogOpen} /> */}
      <div className="py-4 px-4 md:px-8 flex flex-col gap-y-4">
        <div className="flex items-center justify-between">
          <h5 className="font-medium text-xl">Meus Ingredientes</h5>
          <Button onClick={() => {}}>
            <PlusIcon />
            Novo ingrediente
          </Button>
        </div>
        <ScrollArea>
          <div className="flex items-center gap-x-2 p-1">
            {/* <MeetingsSearchFilter /> */}
            {/* <StatusFilter /> */}
            {/* <AgentIdFilter /> */}
            {/* {isAnyFilterModified && (
              <Button variant="outline" onClick={onClearFilters}>
                <XCircleIcon className="size-4" />
                Clear
              </Button>
            )} */}
          </div>
          <ScrollBar orientation="horizontal" />
        </ScrollArea>
      </div>
    </>
  );
}