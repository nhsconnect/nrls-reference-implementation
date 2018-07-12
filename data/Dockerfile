FROM mongo:3.6

WORKDIR /nrls-data
ADD defaultdata defaultdata
ADD cmds cmds

RUN chmod +x /nrls-data/cmds/run.sh
RUN chmod +x /nrls-data/cmds/set_mongodb_users.sh
RUN chmod +x /nrls-data/cmds/set_mongodb_data.sh

CMD ["/nrls-data/cmds/run.sh"]